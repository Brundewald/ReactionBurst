using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace MyProject.ReactionBurst.UI
{
    public interface IUIService
    {
        /// <summary>
        /// Create View and add to Queue for show
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <typeparam name="T">Presenter type</typeparam>
        UniTask<T> ConstructWindowAsync<T>(string viewName) where T : BasePresenter;

        /// <summary>
        /// Show next View from Queue
        /// </summary>
        T ShowWindow<T>() where T : BasePresenter;

        /// <summary>
        /// HideWindow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void HideWindow<T>() where T  : BasePresenter;

        /// <summary>
        /// Completely removes Window and release it's assets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseWindowCompletely<T>() where T  : BasePresenter;

        /// <summary>
        /// Return Presenter by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetWindow<T>() where T : BasePresenter;

        bool IsOpened<T>() where T : BasePresenter;
        bool IsOpened(Type presenterType);
    }

    /// <summary>
    ///  Function for open/close View
    /// </summary>
    public class UIService : IUIService
    {
        private readonly Dictionary<Type, BasePresenter> _forShow;
        private readonly Dictionary<Type, BasePresenter> _opened;
        private readonly UIBuilder _builder;
        
        private Canvas _canvas;

        public UIService(UIBuilder builder)
        {
            _builder = builder;
            _forShow = new Dictionary<Type, BasePresenter>(5);
            _opened = new Dictionary<Type, BasePresenter>(5);
        }
        
        /// <summary>
        /// Create View and add to Queue for show
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <typeparam name="T">Presenter type</typeparam>
        public async UniTask<T> ConstructWindowAsync<T>(string viewName)
            where T : BasePresenter
        {
            if (_forShow.ContainsKey(typeof(T)))
                return _forShow[typeof(T)] as T;
            if(_opened.ContainsKey(typeof(T)))
                return _opened[typeof(T)] as T;
            
            
            var newPresenter = await _builder.BuildPresenter<T>(viewName);
            Assert.IsNotNull(newPresenter, "[BaseDirector] Presenter is null");
            newPresenter.SetName(viewName);
            newPresenter.Hide();
            _forShow.Add(typeof(T), newPresenter);
            return newPresenter;
        }

        /// <summary>
        /// Show next View from Queue
        /// </summary>
        public T ShowWindow<T>() where T : BasePresenter
        {
            var type  = typeof(T);
            
            if (!_forShow.ContainsKey(type))
            {
                Debug.LogWarning("[BaseDirector] OpenView fail: "+ type);
                return default;
            }

            //Debug.Log("[BaseDirector] OpenView "+viewName);
            _opened.Add(type, _forShow[type]);
            ShowView(_forShow[type]);
            _forShow.Remove(type);
            
            return _opened[type] as T;
        }

        /// <summary>
        /// HideWindow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideWindow<T>() where T  : BasePresenter
        {
            var type = typeof(T);
            if(!IsOpened(type))
                return;
            
            HideView(type);
        }
        
        /// <summary>
        /// Completely removes Window and release it's assets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWindowCompletely<T>() where T  : BasePresenter
        {
            var type  = typeof(T);
            if(!IsOpened(type))
                return;
            
            DestroyWindow(_opened[type], _opened[type].GetViewInstance());
            _opened.Remove(type);
        }

        /// <summary>
        /// Return Presenter by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWindow<T>() where T : BasePresenter
        {
            var type = typeof(T);
            if (_forShow.ContainsKey(type))
                return (T) _forShow[type];
            if(_opened.ContainsKey(type))
                return (T) _opened[type];
            return default;
        }

        private void ShowView(BasePresenter presenter)
        {
            presenter.Prepare();
            presenter.Show();
            presenter.Hidden += HideView;
        }

        private void DestroyWindow(BasePresenter presenter, BaseView viewInstance)
        {
            if (presenter == null)
                return;
            
            //Debug.Log("CloseView "+viewInstance.name);
            Object.Destroy(viewInstance);
            _builder.Release(viewInstance);
            if(_opened.ContainsKey(presenter.GetType()))
                _opened.Remove(presenter.GetType());
        }

        public bool IsOpened<T>() where T : BasePresenter
        {
            return IsOpened(typeof(T));
        }
        
        public bool IsOpened(Type presenterType)
        {
            return _opened.ContainsKey(presenterType);
        }

        private void HideView(Type type)
        {
            _opened[type].Hide();
            _forShow.Add(type, _opened[type]);
            _opened.Remove(type);
        }
    }
}
