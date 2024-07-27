using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.AssetsProvider
{
    public interface IAssetsProvider
    {
        public UniTask<IEnumerable<T>> LoadAssetsByLabel<T>(string label) where T : Object;
        public UniTask<T> LoadAssetAsync<T>(AssetReference assetReference) where T : Object;
        public UniTask<T> LoadAssetAsync<T>(string assetGUID) where T : Object;
        public UniTask ReleaseAsset(AssetReference assetReference);
        public UniTask ReleaseAsset(string assetGUID);
        public UniTask ReleaseAllAssets();
    }
}