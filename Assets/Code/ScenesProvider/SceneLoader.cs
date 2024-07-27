using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace MyProject.ReactionBurst.Shared.ScenesProvider
{
    public class SceneLoader : MonoBehaviour
    {
        public event Action Start = delegate { };
        
        public event Action Complete = delegate { };
        
        public event Action<float> ProgressChanged = delegate { };

        private AsyncOperationHandle<SceneInstance> _loadedScene;

        public async UniTask LoadSceneAsync(AssetReference sceneAsset)
        {
            if (_loadedScene.IsValid())
            {
                await SceneManager.UnloadSceneAsync(_loadedScene.Result.Scene);
                Addressables.ReleaseInstance(_loadedScene);
            }
            
            Start();
            _loadedScene = Addressables.LoadSceneAsync(sceneAsset, LoadSceneMode.Additive);
            LoadProgress(_loadedScene).Forget();
            await _loadedScene.ToUniTask(this);
            Assert.IsTrue(_loadedScene.Status == AsyncOperationStatus.Succeeded, "Fail LoadSceneAsync by guid "+sceneAsset.AssetGUID);
            
            var scenePreloader = FindObjectOfType<AScenePreloader>();
            if (scenePreloader != null)
            {
                await scenePreloader.Preload();
                await scenePreloader.WaitForLoad();
            }

            Complete();

            if (scenePreloader != null)
            {
                scenePreloader.OnLoaded();
            }
        }
        
        private async UniTask LoadProgress(AsyncOperationHandle asyncOp)
        {
            while (!asyncOp.IsDone)
            {
                //Debug.Log("LoadProgress "+asyncOp.PercentComplete.ToString("F"));
                ProgressChanged.Invoke(asyncOp.PercentComplete);
                await UniTask.Yield();
            }
        }
    }
}