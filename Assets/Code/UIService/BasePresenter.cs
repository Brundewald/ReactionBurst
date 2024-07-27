using System;
using UnityEngine;

namespace MyProject.ReactionBurst.UI
{
    // <summary>
    /// Base class for all Presenter: link Model and View
    /// </summary>
    public class BasePresenter : IDisposable
    {
        public event Action<Type> Hidden = delegate {  };

        private readonly BaseView _view;
        private readonly BaseModel _model;
        private string _name;
        public string Name => _name;

        protected BasePresenter(BaseView view, BaseModel model)
        {
            _view = view;
            _view.Hiden += OnViewHided;
            _model = model;
            _model.PropertyChanged += UpdateView;
        }

        public virtual void Dispose()
        {
            _view.Hiden -= OnViewHided;
            _model.PropertyChanged -= UpdateView;
        }

        public void SetName(string name) => _name = name;

        public virtual void Prepare() => ShowData();

        private void ShowData() => _view.Prepare(_model);

        private void UpdateView(string data, object value) => _view.UpdateData(data, value);

        public void Show() => _view.gameObject.SetActive(true);

        public void Hide() => _view.gameObject.SetActive(false);

        private void OnViewHided()
        {
            if (ViewClosed())
            {
                Hidden.Invoke(GetType());
            }
        }

        protected virtual bool ViewClosed()
        {
            return _view.gameObject.activeInHierarchy;
        }

        public BaseView GetViewInstance()
        {
            return _view;
        }
    }
}