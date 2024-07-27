using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyProject.ReactionBurst.AudiolizationService
{
    /// <summary>
    /// Класс осуществляет загрузку и выгрузку AudioClip в память с помощью Addressables
    /// </summary>
    public class AddressableAudioClipLoader : IAudioClipLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle<AudioClip>> _loadedClips;

        /// <summary>
        /// В конструкторе создаём новый Dictionary для учета загруженных клипов
        /// и возможности последующей выгрузки их из оперативной памяти по ключу.
        /// </summary>
        public AddressableAudioClipLoader()
        {
            _loadedClips = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
        }
        
        /// <summary>
        /// Загружаем из Addressables необходимый AudioClip и по завершению загрузки вызываем событие для того,
        /// чтобы предупредить высокоуровневую логику о завершении загруски и возвращаем обратно AudioClip
        /// </summary>
        /// <param name="key">Название Addressables для загрузки</param>
        /// <returns>AudioClip
        /// </returns>
        public async UniTask<AudioClip> LoadClipAsync(string key)
        {
            AsyncOperationHandle<AudioClip> loadOperationHandler;
            if (_loadedClips.ContainsKey(key))
            {
                loadOperationHandler = _loadedClips[key];
            }
            else
            {
                loadOperationHandler = GetClipLoadOperationAsync(key);
                _loadedClips.Add(key, loadOperationHandler);
            }
            
            //Debug.Log($"Loading started{key}");
            
            var clip = await loadOperationHandler;
            Assert.IsTrue(loadOperationHandler.Status == AsyncOperationStatus.Succeeded, $"Fail to load AudioClip - {key}");
            
            //Debug.Log($"Loading Completed{clip.name}");
            
            return clip;
        }

        public AsyncOperationHandle<AudioClip> GetClipLoadOperationAsync(string key)
        {
            var loadOperationHandler = Addressables.LoadAssetAsync<AudioClip>(key);
            Assert.IsTrue(loadOperationHandler.IsValid(), $"Not Valid operation for load AudioClip - {key}");
            return loadOperationHandler;
        }

        /// <summary>
        /// По окончании проигрывании трека выгружаем его из памяти по ключу.
        /// </summary>
        /// <param name="key"> Название ключа</param>
        public void Release(string key)
        {
            var clipIsNotNull = _loadedClips.TryGetValue(key, out var clipToRelease);   
            
            if(clipIsNotNull)
            {
                if(clipToRelease.IsValid())
                    Addressables.Release(clipToRelease);
                _loadedClips.Remove(key);
                //Debug.Log($"Clip {key} successfully released!");
            }
            else
            {
                Debug.Log($"Wrong key or {key} already released");
            }
        }
    }
}

