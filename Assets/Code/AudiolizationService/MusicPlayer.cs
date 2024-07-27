using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Shared_Code.SharedConstants;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Zenject;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public interface IMusicPlayer
    {
        event Action AllClipsReleased;

        /// <summary>
        /// Пробрасываем зависимости вне Zenject
        /// </summary>
        /// <param name="audioMixerGroup"></param>
        /// <param name="audioSource"></param>
        void Initialize(AudioMixerGroup audioMixerGroup, AudioSource audioSource);
        
        /// <summary>
        /// Прогреваем звуковую библиотеку
        /// </summary>
        /// <param name="keys"></param>
        UniTask Preload(List<string> keys);

        /// <summary>
        /// Загружает AudioClip по конкретному ключу и добавляет его в список. 
        /// </summary>
        /// <param name="key">Ключ для поиска</param>
        UniTask LoadClipFromKey(string key);

        /// <summary>
        /// Ищет клип в Dictionary по ключу, присваивает его AudioSource и воспроизводит его.
        /// </summary>
        /// <param name="key">Ключ для поиска клипа в Dictionary</param>
        void PlayNow(string key);

        /// <summary>
        /// Начинает воспроизведение клипов в порядке очереди, начиная с нулевого объекта в Dictionary.
        /// </summary>
        void PlayQueued();

        /// <summary>
        /// Позволяет принудительно переключить на следующий трек в очереди, прерывая текущий.
        /// </summary>
        /// <param name="isQueued">"true" - если требуется начать автоматическое воспроизведение в порядке очереди.
        /// "false" - если не требуется</param>
        void NextClip(bool isQueued);

        /// <summary>
        /// Мгновенно останавливает воспроизведение клипа и очереди.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Выгружаем все загруженные аудио клипы.
        /// </summary>
        void ReleaseAllClips();
    }

    /// <summary>
    /// Данный класс обеспечивает управление AudioSource, смену проигрываемого клипа и его остановку
    /// </summary>
    public class MusicPlayer : IMusicPlayer
    {
        private readonly IAudioClipLoader _audioClipLoader;
        private AudioSource _audioSource;
        private readonly Dictionary<string, AudioClip> _clips;
        private List<string> _keys;
        private int _clipCount;
        private string _currentKey;
        private readonly CancellationTokenSource _cts;


        public event Action AllClipsReleased = delegate {  };
        private event Action ClipElapsed = delegate {  };

        /// <summary>
        /// В конструктор передаём AudioMixerGroup, AudioSource и List с ключами для загрузки клипов.
        /// В конструкторе присваивается группа для AudioSource и подписка на события.
        /// </summary>
        /// <param name="audioMixerGroup">Конкретная группа микшера для воспроизведения фоновой музыки</param>
        /// <param name="audioSource">Источник звука для воспроизведения клипов</param>
        /// <param name="audioClipLoader"></param>
        /// <param name="keys">Список ключей, по которым будет производиться загрузка</param>
        public MusicPlayer(IAudioClipLoader audioClipLoader)
        {
            _audioClipLoader = audioClipLoader;
            _clips = new Dictionary<string, AudioClip>();
            _keys = new List<string>();
            _clipCount = 0;
            _cts = new CancellationTokenSource();
        }

        public void Initialize(AudioMixerGroup audioMixerGroup, AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;
            _cts.RegisterRaiseCancelOnDestroy(_audioSource);
        }

        /// <summary>
        /// Прогреваем звуковую библиотеку
        /// </summary>
        /// <param name="keys"></param>
        public async UniTask Preload(List<string> keys)
        {
            foreach (var key in keys)
            {
                if(_clips.ContainsKey(key)) continue;

                var keyToLoad = _audioClipLoader is ResourcesAudioClipLoader
                    ? ResourcePaths.MusicResourcePath + key
                    : key;
                
                var clip = await _audioClipLoader.LoadClipAsync(keyToLoad)
                    .AttachExternalCancellation(_cts.Token);
                
                Assert.IsNotNull(clip, "Clip not found!");
                _clips.Add(key, clip);   
                _keys.Add(key);
            }
        }

        /// <summary>
        /// Загружает AudioClip по конкретному ключу и добавляет его в список. 
        /// </summary>
        /// <param name="key">Ключ для поиска</param>
        public async UniTask LoadClipFromKey(string key)
        {
            if(_clips.ContainsKey(key))
                return;
            
            var clip = await _audioClipLoader.LoadClipAsync(key).AttachExternalCancellation(_cts.Token);
            Assert.IsNotNull(clip, $"Clip with key - {key} not found");
            _clips.Add(key, clip);
            Debug.Log($"Clip loaded {key}");
        }

        /// <summary>
        /// Ищет клип в Dictionary по ключу, присваивает его AudioSource и воспроизводит его.
        /// </summary>
        /// <param name="key">Ключ для поиска клипа в Dictionary</param>
        public async void PlayNow(string key)
        {
            if (_audioSource.isPlaying && _clips.ContainsKey(key))
            {
                await FadeOut();
                PlayClip(key, false);
            }
            else if (_clips.ContainsKey(key))
            {
                PlayClip(key, false);
            }
            else
            {
                Reload();
            }
        }

        /// <summary>
        /// Начинает воспроизведение клипов в порядке очереди, начиная с нулевого объекта в Dictionary.
        /// </summary>
        public void PlayQueued()
        {
            ClipElapsed += PlayQueued;
            NextClip(true);
        }

        /// <summary>
        /// Позволяет принудительно переключить на следующий трек в очереди, прерывая текущий.
        /// </summary>
        /// <param name="isQueued">"true" - если требуется начать автоматическое воспроизведение в порядке очереди.
        /// "false" - если не требуется</param>
        public void NextClip(bool isQueued)
        {
            if (CurrentIndex() >= _keys.Count)
            {
                Debug.Log("All clips played, use Play() to reload");
                if (isQueued)
                {
                    ClipElapsed -= PlayQueued;
                    ReleaseAllClips();
                }
            }
            else
            {
                if (_audioSource.clip == null)
                {
                    PlayClip(_keys[_clipCount], isQueued);
                }
                else
                {
                    SwitchClip(isQueued);
                }
            }
        }

        /// <summary>
        /// Мгновенно останавливает воспроизведение клипа и очереди.
        /// </summary>
        public void Stop()
        {
            if (_currentKey is null)
            {
                Debug.Log("There is no clip to stop yet!");
                return;
            }
            
            if (_clips.Count == 0)
            {
                return;
            }
            
            _audioSource.Stop();

            var keyToRelease = _audioClipLoader is ResourcesAudioClipLoader
                ? ResourcePaths.MusicResourcePath + _currentKey
                : _currentKey;
            _audioClipLoader.Release(keyToRelease);
            
            
            _clips.Remove(_currentKey);
            _currentKey = null;
        }

        /// <summary>
        /// Выгружаем все клипы из Dictionary.
        /// </summary>
        public void ReleaseAllClips()
        {
            _audioSource.Stop();
            
            foreach (var key in _keys)
            {
                _audioClipLoader.Release(key);
            }
            
            _keys.Clear();
            _clips.Clear();
        }

        private async void PlayClip(string key, bool isQueued)
        {
            _currentKey = key;
            _clips.TryGetValue(key, out var clip);
            Assert.IsNotNull(clip, "Clip not found!");
            if(!_cts.IsCancellationRequested)
            {
                _audioSource.volume = 0.01f;
                _audioSource.clip = clip;
                _audioSource.Play();
            }
            Debug.Log($"{key} now is playing");
           
            await FadeIn();
          
            if(isQueued)
            {
                await AudioSourceStatus();
            }
        }

        private async void SwitchClip(bool isQueued)
        {
            if(_audioSource.isPlaying)
            {
                await FadeOut();
                IncreaseCount();
                PlayClip(_keys[_clipCount], isQueued);
            }
            else
            {
                IncreaseCount();
                PlayClip(_keys[_clipCount], isQueued);
            }
        }

        private void Reload()
        {
            Debug.Log("AllClipReleased! Reloading.");
            _audioSource.clip = null;
            SetCountToZero();
            AllClipsReleased.Invoke();
        }

        private async UniTask FadeOut()
        {
            //await _audioSource.DOFade(0f, 1f).WithCancellation(_cts.Token);
            
            if(!_cts.IsCancellationRequested)
                Stop();
        }

        private async UniTask FadeIn()
        {
            //await _audioSource.DOFade(1f, 1f).WithCancellation(_cts.Token);
        }

        private async UniTask AudioSourceStatus()
        {
            while (_audioSource.isPlaying && !_cts.Token.IsCancellationRequested)
            {
                Debug.Log($"{_currentKey} is playing");
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            var clipDuration = (float)_audioSource.clip.samples / _audioSource.clip.frequency;
            var clipIsRunOut = Mathf.Approximately(_audioSource.time, clipDuration);
            
            if(clipIsRunOut)
            {
                ClipElapsed.Invoke();
            }
        }

        private void IncreaseCount()
        {
            _clipCount++;
        }

        private void SetCountToZero()
        {
            _clipCount = 0;
        }

        private int CurrentIndex()
        {
            return _clipCount + 1;
        }
    }
}