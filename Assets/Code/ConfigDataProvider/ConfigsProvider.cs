
using System;
using System.Collections.Generic;
using Code.Infrastructure.AssetsProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace MyProject.Config
{
    public interface IConfigsProvider
    {
        UniTask<T> GetConfigAsync<T>() where T : ScriptableObject;
    }

    public class ConfigsProvider : IConfigsProvider, IInitializable
    {
        private readonly IAssetsProvider _assetsProvider;
        
        private Dictionary<Type, ScriptableObject> _configs = new Dictionary<Type, ScriptableObject>();
        private bool _isInitialized;


        public ConfigsProvider(IAssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
        }

        public async void Initialize()
        {
            var configs = await _assetsProvider.LoadAssetsByLabel<ScriptableObject>("Configs");

            foreach (var config in configs)
            {
                _configs.Add(config.GetType(), config);
            }
            
            _isInitialized = true;
        }

        public async UniTask<T> GetConfigAsync<T>() where T : ScriptableObject
        {
            await UniTask.WaitUntil(() => _isInitialized);
            return _configs.ContainsKey(typeof(T)) ? _configs[typeof(T)] as T : null;
        }
    }
}
