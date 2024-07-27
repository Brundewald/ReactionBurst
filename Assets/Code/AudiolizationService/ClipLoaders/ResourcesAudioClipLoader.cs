using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace MyProject.ReactionBurst.AudiolizationService
{
    /// <summary>
    /// Класс осуществляет загрузку и выгрузку AudioClip в память с помощью Resources
    /// </summary>
    public class ResourcesAudioClipLoader : IAudioClipLoader, IInitializable
    {
        private Dictionary<string, AudioClip> _loadedClips;


        public void Initialize()
        {
            _loadedClips = new Dictionary<string, AudioClip>();
        }

        /// <summary>
        /// Загружаем из Resources необходимый AudioClip и по завершению загрузки вызываем событие для того,
        /// чтобы предупредить высокоуровневую логику о завершении загруски и возвращаем обратно AudioClip
        /// </summary>
        /// <param name="key">Название клипа для загрузки</param>
        /// <returns>AudioClip
        /// </returns>
        public async UniTask<AudioClip> LoadClipAsync(string key)
        {
            AudioClip clip;
            if (_loadedClips.ContainsKey(key))
            {
                clip = _loadedClips[key];
            }
            else
            {
                clip = await GetClipLoadOperationAsync(key);
                _loadedClips.Add(key, clip);
            }
            
            Assert.IsNotNull(clip, $"Fail to load AudioClip - {key}");
            
            //Debug.Log($"Loading Completed{clip.name}");
            
            return clip;
        }

        public async UniTask<AudioClip> GetClipLoadOperationAsync(string key)
        {
            var audioClip = await Resources.LoadAsync<AudioClip>(key) as AudioClip;
            return audioClip;
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
                _loadedClips.Remove(key);
                clipToRelease.UnloadAudioData();
                //Debug.Log($"Clip {key} successfully released!");
            }
            else
            {
                Debug.Log($"Wrong key or {key} already released");
            }
        }
    }
}