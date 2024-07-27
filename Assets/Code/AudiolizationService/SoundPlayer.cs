using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;
using Shared_Code.SharedConstants;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public interface ISoundPlayer
    {
        /// <summary>
        /// Пробрасываем зависимости вне Zenject
        /// </summary>
        /// <param name="audioMixerGroup"></param>
        void Initialize(AudioMixerGroup soundAudioMixer);
        
        /// <summary>
        /// Прогреваем звуковую библиотеку
        /// </summary>
        /// <param name="keys"></param>
        UniTask Preload(List<string> clipKeys);

        /// <summary>
        /// Проверяет go на наличие AudioSource и проигрывается ли звук сейчас или нет, и присваевает ему загруженный клип.
        /// При создании AudioSource добавляется в лист и присваевает необходимую AudioMixerGroup
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key"></param>
        void Play(GameObject go, string key, Action<GameObject> callback);

        /// <summary>
        /// Проверяет go на наличие AudioSource и проигрывается ли звук сейчас или нет, и присваевает ему загруженный клип.
        /// При создании AudioSource добавляется в лист и присваевает необходимую AudioMixerGroup
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        UniTask PlayOneShot(GameObject go, string key, Action<GameObject> callback);

        /// <summary>
        /// Останавливает все проигрываемые sfx и звапускает нужный. 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для запуска клипа</param>
        void PlayInterrupted(GameObject go, string key, Action<GameObject> callback);

        /// <summary>
        /// Запускает необходимый трек в нужных координатах для создания объёмного звука.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для воспроизведения</param>
        void PlayAt(GameObject go, string key, Action<GameObject> callback);

        /// <summary>
        /// Запускает аудиоклип в режиме loop. 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для запуска клипа</param>
        void PlayLooped(GameObject go, string key);

        /// <summary>
        /// Продолжает воспроизведение зацклинного звука
        /// </summary>
        /// <param name="key">Ключ для запуска клипа</param>
        void ResumeLooped(string key);

        /// <summary>
        /// Останавливает воспроизведение зацклинного звука
        /// </summary>
        /// <param name="key">Ключ для запуска клипа</param>
        void PauseLooped(string key);

        /// <summary>
        /// Останавливает воспроизведение закленного SFX полностью
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        void StopLoopedSFXCompletely(string key, Action<GameObject> callback);

        /// <summary>
        /// Выгружает все клипы из памяти и удаляет все объекты с AudioSource со сцены.
        /// </summary>
        void ReleaseAllClips();
    }

    /// <summary>
    /// Метод позволяет проигрывать и останавливать клипы с звуковыми эффектами.
    /// </summary>
    public class SoundPlayer : ISoundPlayer
    {
        private readonly IAudioClipLoader _audioClipLoader;
        private AudioMixerGroup _audioMixerGroup;
        private readonly Dictionary<string, AudioClip> _clips;
        private readonly Dictionary<GameObject, AudioSource> _audioSources;
        private readonly Dictionary<string, AudioSource> _loopedAudioSources;
        private readonly float _spatialBlendCoefficient;
        private readonly int _soundObjectsCount;
        private bool _audioSourceStoped;

        public SoundPlayer(IAudioClipLoader audioClipLoader)
        {
            _audioClipLoader = audioClipLoader;
            _clips = new Dictionary<string, AudioClip>();
            _audioSources = new Dictionary<GameObject, AudioSource>();
            _loopedAudioSources = new Dictionary<string, AudioSource>();
        }

        public void Initialize(AudioMixerGroup soundAudioMixer)
        {
            _audioMixerGroup = soundAudioMixer;
        }

        public async UniTask Preload(List<string> clipKeys)
        {
            foreach (var key in clipKeys)
            {
                var cts = new CancellationTokenSource();
                await GetClip(key, cts.Token);
            }
        }

        /// <summary>
        /// Проверяет go на наличие AudioSource и проигрывается ли звук сейчас или нет, и присваевает ему загруженный клип.
        /// При создании AudioSource добавляется в лист и присваевает необходимую AudioMixerGroup
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key"></param>
        public void Play(GameObject go, string key, Action<GameObject> callback)
        {
            PlayClip(go, key, callback);
        }

        /// <summary>
        /// Проверяет go на наличие AudioSource и проигрывается ли звук сейчас или нет, и присваевает ему загруженный клип.
        /// При создании AudioSource добавляется в лист и присваевает необходимую AudioMixerGroup
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public async UniTask PlayOneShot(GameObject go, string key, Action<GameObject> callback)
        {
            await PlayOneShotClip(go, key, callback);
        }

        /// <summary>
        /// Останавливает все проигрываемые sfx и звапускает нужный. 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для запуска клипа</param>
        public void PlayInterrupted(GameObject go, string key, Action<GameObject> callback)
        {
            foreach (var pair in _audioSources)
            {
                if (pair.Value.isPlaying)
                {
                    pair.Value.Stop();
                }
            }

            PlayClip(go, key, callback);
        }

        /// <summary>
        /// Запускает необходимый трек в нужных координатах для создания объёмного звука.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для воспроизведения</param>
        public void PlayAt(GameObject go, string key, Action<GameObject> callback)
        {
            PlayClip(go, key, callback);
        }

        /// <summary>
        /// Запускает аудиоклип в режиме loop. 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="key">Ключ для запуска клипа</param>
        public void PlayLooped(GameObject go, string key)
        {
            PlayClip(go, key, null, true);
        }

        /// <summary>
        /// Продолжает воспроизведение зацклинного звука
        /// </summary>
        /// <param name="key">Ключ для запуска клипа</param>
        public void ResumeLooped(string key)
        {
            _loopedAudioSources[key].Play();
        }

        /// <summary>
        /// Останавливает воспроизведение зацклинного звука
        /// </summary>
        /// <param name="key">Ключ для запуска клипа</param>
        public void PauseLooped(string key)
        {
            _loopedAudioSources[key].Pause();
        }

        /// <summary>
        /// Останавливает воспроизведение закленного SFX полностью
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void StopLoopedSFXCompletely(string key, Action<GameObject> callback)
        {
            var audioSource = _loopedAudioSources[key];
            audioSource.Stop();
            _loopedAudioSources.Remove(key);
            callback.Invoke(audioSource.gameObject);
        }

        /// <summary>
        /// Выгружает все клипы из памяти и удаляет все объекты с AudioSource со сцены.
        /// </summary>
        public void ReleaseAllClips()
        {
            foreach (var clip in _clips)
            {
                var keyToRelease = _audioClipLoader is ResourcesAudioClipLoader
                    ? ResourcePaths.AudioResourcePath + clip.Key
                    : clip.Key;
                
                _audioClipLoader.Release(keyToRelease);
            }

            foreach (var pair in _audioSources)
            {
                pair.Value.Stop();
            }

            _clips.Clear();
            _audioSources.Clear();
            _loopedAudioSources.Clear();
        }

        private void PlayClip(GameObject go, string key, Action<GameObject> callback = null, bool isLooped = false)
        {
            var audioSource = PrepareSoundSource(go, isLooped, key);

            SetAudioSource(audioSource, key, callback);
        }

        private async UniTask PlayOneShotClip(GameObject go, string key, Action<GameObject> callback)
        {
            var audioSource = PrepareSoundSource(go);
            var cts = new CancellationTokenSource();
            cts.RegisterRaiseCancelOnDestroy(audioSource);
            var clip = await GetClip(key, cts.Token);
            audioSource.PlayOneShot(clip);
            await UniTask
                .WaitUntil(() => !audioSource.isPlaying, cancellationToken: cts.Token)
                .SuppressCancellationThrow();
            callback.Invoke(go);
        }

        private async UniTask<AudioClip> GetClip(string key, CancellationToken cancellationToken)
        {
            AudioClip clip;
            
            if (!_clips.ContainsKey(key))
            {
                var keyToLoad = _audioClipLoader is ResourcesAudioClipLoader
                    ? ResourcePaths.AudioResourcePath + key
                    : key;
                
                clip = await _audioClipLoader.LoadClipAsync(keyToLoad)
                        .AttachExternalCancellation(cancellationToken);
                
                Assert.IsNotNull(clip, "Clip not in the dictionary");
                _clips.TryAdd(key, clip);
            }
            else
            {
                clip = _clips[key];
            }

            return clip;
        }

        private AudioSource PrepareSoundSource(GameObject go, bool isLooped = false, string key = "")
        {
            if (!go.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource ??= go.AddComponent<AudioSource>();
            }

            if (!_audioSources.Keys.Contains(go) && !isLooped)
            {
                _audioSources.Add(go, audioSource);
            }

            if (isLooped)
            {
                audioSource.loop = true;
                _loopedAudioSources.Add(key, audioSource);
            }

            return audioSource;
        }

        private async void SetAudioSource(AudioSource audioSource, string key, Action<GameObject> callback = null)
        {
            var cts = new CancellationTokenSource();
            cts.RegisterRaiseCancelOnDestroy(audioSource);
            var clip = await GetClip(key, cts.Token);

            if (clip is null)
            {
                Debug.Log("You tried to load the same key before release, or incorrect key");
                return;
            }

            if (audioSource.outputAudioMixerGroup is null
                || !audioSource.outputAudioMixerGroup.Equals(_audioMixerGroup))
            {
                audioSource.outputAudioMixerGroup = _audioMixerGroup;
            }

            audioSource.panStereo = audioSource.transform.position.x;
            audioSource.clip = clip;

            audioSource.Play();

            await UniTask
                .WaitUntil(() => !audioSource.isPlaying, cancellationToken: cts.Token)
                .SuppressCancellationThrow();

            callback?.Invoke(audioSource.gameObject);
        }
    }
}