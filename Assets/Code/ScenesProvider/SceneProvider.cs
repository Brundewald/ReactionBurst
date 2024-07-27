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
    public class SceneProvider : MonoBehaviour
    {
        [SerializeField] 
        private ScenesData _scenes;

        [SerializeField] 
        private SceneLoader _loader;
      
        [SerializeField] 
        private SceneAssetReference _loadingScene;

        private ASceneData _sceneData;
        
        private string _loadedSceneName;
        
        private AsyncOperationHandle<SceneInstance> _loadingSceneHandle;
        
        public event Action<float> DownloadProgressChanged = delegate { };
        
        public event Action<float> LoadProgressChanged =delegate { };

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            _loadedSceneName = sceneName;
            LoadingSceneAsync(_loadingScene).Forget();
        }

        public void LoadWithoutLoading(string sceneName)
        {
            _loadedSceneName = sceneName;
            LoadWithoutLoading();
        }
        
        public void LoadScene(string sceneName, SceneAssetReference loadingScene)
        {
            _loadedSceneName = sceneName;
            LoadingSceneAsync(loadingScene).Forget();
        }

        private async UniTask LoadingSceneAsync(SceneAssetReference loadingScene)
        {
            _loadingSceneHandle = Addressables.LoadSceneAsync(loadingScene);
            await _loadingSceneHandle.ToUniTask(this);
            LoadScene();
        }

        private void LoadScene()
        {
            var asset = _scenes.GetByName(_loadedSceneName);
            Assert.IsNotNull(asset, "[SceneProvider] Not find scene by name "+_loadedSceneName);
            _loader.Complete += LoaderOnLoadingComplete;
            _loader.LoadSceneAsync(asset).Forget();
        }

        private void LoadWithoutLoading()
        {
            var asset = _scenes.GetByName(_loadedSceneName);
            Assert.IsNotNull(asset, "[SceneProvider] Not find scene by name "+_loadedSceneName);
            _loader.Complete += UnloadCurrentScene;
            _loader.LoadSceneAsync(asset).Forget();
        }

        private void LoaderOnLoadingComplete()
        {
            _loader.Complete -= LoaderOnLoadingComplete;
            SceneManager.UnloadSceneAsync(_loadingSceneHandle.Result.Scene);
            Addressables.ReleaseInstance(_loadingSceneHandle);
        }

        private void UnloadCurrentScene()
        {
            _loader.Complete -= UnloadCurrentScene;
            SceneManager.UnloadSceneAsync(0, UnloadSceneOptions.None);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            Assert.IsNotNull(_scenes);
            Assert.IsNotNull(_loader);
        }
#endif

        public async UniTask DownloadGame(string gameId)
        {
            var key = gameId + "/Main";
            _loadedSceneName = gameId;
            //Loading scene
            _loadingSceneHandle = Addressables.LoadSceneAsync(_loadingScene);
            await _loadingSceneHandle.ToUniTask(this);
            //Download bundles
            var getSizeHandle = Addressables.GetDownloadSizeAsync(key);
            await getSizeHandle;
            //Debug.Log("DownloadGame "+gameId+" = "+ getSizeHandle.Result);
            if (getSizeHandle.Result > 0)
            {
                var getBundlesHandle = Addressables.DownloadDependenciesAsync(key);
                DownloadProgress(getBundlesHandle).Forget();
                await getBundlesHandle;
                //Debug.Log("Game "+gameId+" downloaded");
                Addressables.Release(getBundlesHandle);
            }
            _loader.ProgressChanged += progress =>
            {
                LoadProgressChanged(progress);
            };
            //Load game scene
            LoadScene();
        }
        
        private async UniTask DownloadProgress(AsyncOperationHandle asyncOp)
        {
            while (!asyncOp.IsDone)
            { 
                //Debug.Log("DownloadProgress "+asyncOp.PercentComplete.ToString("F"));
                DownloadProgressChanged.Invoke(asyncOp.PercentComplete);
                await UniTask.Yield();
            }
        }

        public void SetSceneData(object data)
        {
            _sceneData = new ASceneData();
            _sceneData.SetData(data);
        }

        public T GetSceneData<T>() where T : class
        {
            _sceneData = new ASceneData();
            return _sceneData.GetData<T>();
        }

        public bool HasSceneData => _sceneData != null;
    }
}