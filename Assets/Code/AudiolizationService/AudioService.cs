using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.SaveLoad;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public interface IAudioService
    {
        public UniTask Preload();
        public void SetMusicEnabled(bool state);
        public void SetSoundsEnabled(bool state);
        public UniTask PlaySound(string clipKey);
        public UniTaskVoid PlayMusic(string musicKey);
        public void StopMenuMusic();
        public void PlayLoopedSFX(string key);
        public void ResumeLoopedSFX(string key);
        public void PauseLoopedSFX(string key);
        public void StopLoopedSFXCompletely(string key);
        public void Release();
        public bool MusicEnabled { get; }
        public bool SFXEnabled { get; }
    }
    
    public class AudioService : MonoBehaviour, IAudioService
    {
        private const string MusicVolumeExpose = "MusicVolume";
        private const string SoundVolumeExpose = "SFXVolume";
        private const string SoundMode = nameof(SoundMode);
        private const string MusicMode = nameof(MusicMode);
        private const string IsInitialized = nameof(IsInitialized);

        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioMixerGroup _musicAudioMixer;
        [SerializeField] private AudioMixerGroup _soundAudioMixer;

        [Space(10)]
        [SerializeField] private List<GameObject> _soundsSourceGameObjects;

        private IMusicPlayer _musicPlayer;
        private ISoundPlayer _soundPlayer;
        private AudioServiceConfig _audioServiceConfig;
        private CancellationTokenSource _cts;
        private ISaveLoadService _saveDataService;
        private List<GameObject> _usingSources = new List<GameObject>();
        private string _currentPlayingMusic = string.Empty;
        private bool _sfxEnabled;
        private bool _musicEnabled;

        public bool MusicEnabled => _musicEnabled;
        public bool SFXEnabled=> _sfxEnabled;

        [Inject]
        public void Constructor(ISaveLoadService saveLoadService, 
            IMusicPlayer musicPlayer, 
            ISoundPlayer soundPlayer, 
            AudioServiceConfig audioServiceConfig)
        {
            _audioServiceConfig = audioServiceConfig;
            _saveDataService = saveLoadService;
            _musicPlayer = musicPlayer;
            _soundPlayer = soundPlayer;
        }
        
        private void Start()
        {
            _cts = new CancellationTokenSource();
            
            if (!_saveDataService.GetData<bool>(IsInitialized))
            {
                _saveDataService.SetData(MusicMode, true);
                _saveDataService.SetData(SoundMode, true);
                _saveDataService.SetData(IsInitialized, true);
            }

            SetMusicEnabled(_saveDataService.GetData<bool>(MusicMode));
            SetSoundsEnabled(_saveDataService.GetData<bool>(SoundMode));

            _cts.RegisterRaiseCancelOnDestroy(_musicAudioSource);
            Initialize();
        }

        private void Initialize()
        {
            _musicPlayer.Initialize(_musicAudioMixer, _musicAudioSource);
            _soundPlayer.Initialize(_soundAudioMixer);
            DontDestroyOnLoad(this);
        }

        public async UniTask Preload()
        {
            await _soundPlayer.Preload(_audioServiceConfig.SFXKeys);
            await _musicPlayer.Preload(_audioServiceConfig.MusicKeys);
        }

        public void SetMusicEnabled(bool state)
        {
            _musicEnabled = state;
            _saveDataService.SetData(MusicMode, state);
            _musicAudioMixer.audioMixer.SetFloat(MusicVolumeExpose, state ? 0f : -80f);
        }

        public void SetSoundsEnabled(bool state)
        {
            _sfxEnabled = state;
            _saveDataService.SetData(SoundMode, state);
            _soundAudioMixer.audioMixer.SetFloat(SoundVolumeExpose, state ? 0f : -80f);
        }

        public async UniTask PlaySound(string clipKey)
        {
            var source = GetAudioSource();
            await _soundPlayer.PlayOneShot(source, clipKey, OnAudioSourceStopped);
            _soundsSourceGameObjects.Remove(source);
            _usingSources.Add(source);
        }

        public async UniTaskVoid PlayMusic(string musicKey)
        {
            if (_currentPlayingMusic.Equals(musicKey))
                return;
            if (!string.IsNullOrEmpty(_currentPlayingMusic))
            {
                _musicPlayer.Stop();
            }

            await _musicPlayer.LoadClipFromKey(musicKey);
            _musicPlayer.PlayNow(musicKey);
            _currentPlayingMusic = musicKey;
        }

        public void StopMenuMusic()
        {
            _musicPlayer.Stop();
        }

        public void PlayLoopedSFX(string key)
        {
            var source = GetAudioSource();
            _soundPlayer.PlayLooped(source, key);
            _soundsSourceGameObjects.Remove(source);
            _usingSources.Add(source);
        }

        public void ResumeLoopedSFX(string key)
        {
            _soundPlayer.ResumeLooped(key);
        }

        public void PauseLoopedSFX(string key)
        {
            _soundPlayer.PauseLooped(key);
        }

        public void StopLoopedSFXCompletely(string key)
        {
            _soundPlayer.StopLoopedSFXCompletely(key, OnAudioSourceStopped);
        }

        public void Release()
        {
            _soundPlayer.ReleaseAllClips();
            _musicPlayer.ReleaseAllClips();
        }

        private void OnAudioSourceStopped(GameObject go)
        {
            _usingSources.Remove(go);
            _soundsSourceGameObjects.Add(go);
        }

        private GameObject GetAudioSource()
        {
            var source = _soundsSourceGameObjects.FirstOrDefault();
            if (source is null)
            {
                source = CreateNewSource();
            }

            return source;
        }

        private GameObject CreateNewSource()
        {
            var audioSource = new GameObject($"AudioSource", typeof(AudioSource));
            audioSource.transform.SetParent(transform);
            _soundsSourceGameObjects.Add(audioSource.gameObject);
            return audioSource.gameObject;
        }
    }
}