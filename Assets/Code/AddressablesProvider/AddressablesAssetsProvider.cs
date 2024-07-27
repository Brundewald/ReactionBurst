using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Code.Infrastructure.AssetsProvider
{
    public class AddressablesAssetsProvider : IAssetsProvider
    {
        private Dictionary<string, AsyncOperationHandle> _asyncOperationHandles = new Dictionary<string, AsyncOperationHandle>();

        public async UniTask<IEnumerable<T>> LoadAssetsByLabel<T>(string label) where T : Object
        {
            var completeCash = await TryGetCash<IEnumerable<T>>(label);
            
            if(completeCash != null)
                return completeCash;
            
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            completeCash = await LoadHandleAsync(label, handle);
            return completeCash;
        }

        public async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference) where T : Object
        {
            var assetGUID = assetReference.AssetGUID;
            var completeCash = await TryGetCash<T>(assetGUID);
            
            if (completeCash) 
                return completeCash;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetReference);
            completeCash = await LoadHandleAsync(assetReference.AssetGUID, handle);
            return completeCash;
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetGUID) where T : Object
        {
            var completeCash = await TryGetCash<T>(assetGUID);
            
            if (completeCash) 
                return completeCash;
            
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetGUID);

            return await LoadHandleAsync(assetGUID, handle);
        }

        public async UniTask ReleaseAsset(AssetReference assetReference)
        {
            ReleaseOperationHandle(assetReference.AssetGUID);
        }

        public async UniTask ReleaseAsset(string assetGUID)
        {
            ReleaseOperationHandle(assetGUID);
        }

        public UniTask ReleaseAllAssets()
        {
            foreach (var (key, handle) in _asyncOperationHandles)
            {
                Addressables.Release(handle);
            }
            
            _asyncOperationHandles.Clear();
            return UniTask.CompletedTask;
        }

        private void ReleaseOperationHandle(string assetGUID)
        {
            Assert.IsTrue(_asyncOperationHandles.ContainsKey(assetGUID), 
                "You trying to release an asset that is not loaded yet");
            
            if (!_asyncOperationHandles.ContainsKey(assetGUID)) 
                return;
            
            Addressables.Release(_asyncOperationHandles[assetGUID]);
            _asyncOperationHandles.Remove(assetGUID);
        }

        private async UniTask<T> LoadHandleAsync<T>(string assetGUID, AsyncOperationHandle<T> handle)
        {
            _asyncOperationHandles.Add(assetGUID, handle);
            
            float progress = handle.PercentComplete;

            T result = await handle;
            
            Assert.IsTrue(handle.IsDone, "Operation handle is not done");

            return result;
        }

        private async UniTask<T> TryGetCash<T>(string assetGUID) where T : class
        {
            if (!_asyncOperationHandles.ContainsKey(assetGUID))
            {
                return default;
            }

            var handle = _asyncOperationHandles[assetGUID];
            
            await UniTask.WaitUntil(() => handle.IsDone);
            
            return handle.Result as T;
        }
    }
}