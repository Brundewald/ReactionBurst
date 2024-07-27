using System;
using System.Collections.Generic;
using Code.Infrastructure.AssetsProvider;
using Cysharp.Threading.Tasks;
using MyProject.Config;
using MyProject.UIService.Resource;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using Zenject;
using Object = UnityEngine.Object;

namespace MyProject.ReactionBurst.UI
{
    /// <summary>
    /// Создает go с View и Presenter к нему
    /// </summary>
    public class UIBuilder: IInitializable
    {
        private readonly IConfigsProvider _configsProvider;
        private readonly IAssetsProvider _assetsProvider;
        private UIPrefabProvider _prefabProvider;
        private CanvasProvider _canvas;
        
        /// <summary>
        /// Хранит в себе данные о инстанцированных Презентерах, View и PrefabReference
        /// </summary>
        private Dictionary<BaseView, AssetReference> _viewCache = new Dictionary<BaseView, AssetReference>();

        public UIBuilder(IConfigsProvider configsProvider, IAssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
            _configsProvider = configsProvider;
        }

        public async void Initialize()
        {
            _prefabProvider = await _configsProvider.GetConfigAsync<UIPrefabProvider>();
            var canvasPrefabReference = _prefabProvider.GetPrefabReference<CanvasProvider>();
            var canvasPrefab = await _assetsProvider.LoadAssetAsync<GameObject>(canvasPrefabReference);
            _canvas = Object.Instantiate(canvasPrefab).GetComponent<CanvasProvider>();
        }

        /// <summary>
        /// Создает Presenter, назначает ему View и Model 
        /// </summary>
        /// <typeparam name="TPresenter"></typeparam>
        /// <returns>Созданный Presenter</returns>
        public async UniTask<TPresenter> BuildPresenter<TPresenter>(string name) 
            where TPresenter: BasePresenter
        {
            await UniTask.WaitWhile(() => _canvas is null);
            
            var parameters = typeof(TPresenter).GetConstructors()[0].GetParameters();

            BaseView view = null;
            BaseModel model = null;
            
            foreach (var parameter in parameters)
            {
                var type = parameter.ParameterType;
                if (type.IsSubclassOf(typeof(BaseView)))
                {
                    var prefabReference = _prefabProvider.GetPrefabReference(type);
                    var viewPrefab = await _assetsProvider.LoadAssetAsync<GameObject>(prefabReference);
                    var viewInstance = Object.Instantiate(viewPrefab, _canvas.transform, false);
                    viewInstance.name = name;
                    view = viewInstance.GetComponent<BaseView>();
                    Assert.IsNotNull(view, $"View is null  for {type}");
                    _viewCache.Add(view, prefabReference);
                }
                if (type.IsSubclassOf(typeof(BaseModel)))
                {
                    model = Activator.CreateInstance(type) as BaseModel;
                    Assert.IsNotNull(model, $"Model is null for {type}");
                }
            }
            
            var basePresenter = (TPresenter) Activator.CreateInstance(typeof(TPresenter),view, model);
            Assert.IsNotNull(basePresenter, "basePresenter is null");
            
            return basePresenter;
        }

        public void Release(BaseView view)
        {
            var prefabReference = _viewCache[view];
            _viewCache.Remove(view);
            Object.Destroy(view.gameObject);
            _assetsProvider.ReleaseAsset(prefabReference);
        }
    }
}