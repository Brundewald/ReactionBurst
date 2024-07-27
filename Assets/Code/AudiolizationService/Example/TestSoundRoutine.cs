using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public class TestSoundRoutine: MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [SerializeField] private List<string> _keys;
        [SerializeField] private List<GameObject> _objects;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _playInterrupted;
        [SerializeField] private Button _playAt;
        [SerializeField] private Button _releaseAllClips;
        [SerializeField] private Button _playLoopedButton;
        [SerializeField] private Button _pauseLoopedButton;
        [SerializeField] private Button _resumeLoopedButton;
        [SerializeField] private Button _stopLoopedButton;
        [SerializeField] private bool _loadFromResources;

        private SoundPlayer _soundPlayer;
        private int _count;
        private List<GameObject> _usingSources = new List<GameObject>();

        private void Awake()
        {
            IAudioClipLoader audioClipLoader;
            if (_loadFromResources)
                audioClipLoader = new ResourcesAudioClipLoader();
            else
                audioClipLoader = new AddressableAudioClipLoader();
            
            _soundPlayer = new SoundPlayer(audioClipLoader);
            _soundPlayer.Initialize(_audioMixerGroup);
            if(_playButton!= null)
                _playButton.onClick.AddListener(PlayClip);
            if(_playInterrupted != null)
                _playInterrupted.onClick.AddListener(PlayInterrupted);
            if(_playAt != null)
                _playAt.onClick.AddListener(PlayAt);
            if(_releaseAllClips != null)
                _releaseAllClips.onClick.AddListener(ReleaseAllClips);
            if (_playLoopedButton != null) 
                _playLoopedButton.onClick.AddListener(PlayLooped);  
            if(_pauseLoopedButton != null)
                _pauseLoopedButton.onClick.AddListener(PauseLooped); 
            if(_resumeLoopedButton != null)
                _resumeLoopedButton.onClick.AddListener(ResumeLooped);
            if(_stopLoopedButton != null)
                _stopLoopedButton.onClick.AddListener(StopLoopedCompletely);
            _count = 0;
        }

        private void PlayLooped()
        {
            var source = GetAudioSource();
            _soundPlayer.PlayLooped(source, _keys[_count]);
            _objects.Remove(source);
            _usingSources.Add(source);
            if (_count < _keys.Count)
            {
                _count++;
            }
        }

        private void PauseLooped()
        {
            _soundPlayer.PauseLooped(_keys[_count]);
        }

        private void ResumeLooped()
        {
            _soundPlayer.ResumeLooped(_keys[_count]);
        }

        private void StopLoopedCompletely()
        {
            if (_count > 0)
            {
                _count--;
            }
            _soundPlayer.StopLoopedSFXCompletely(_keys[_count], OnAudioSourceStopped);
        }

        private void PlayClip()
        {
            var source = GetAudioSource();
            _soundPlayer.Play(source, _keys[_count], OnAudioSourceStopped);
            _count++;
            if (_count >= _keys.Count)
            {
                _count = 0;
            }
        }

        private void PlayInterrupted()
        {
            var source = GetAudioSource();
            _soundPlayer.PlayInterrupted(source, _keys[_count], OnAudioSourceStopped);
            _count++;
            if (_count >= _keys.Count)
            {
                _count = 0;
            }
        }

        private void PlayAt()
        {
            var source = GetAudioSource();
            _soundPlayer.PlayAt(source, _keys[_count], OnAudioSourceStopped);
            _count++;
            if (_count >= _keys.Count)
            {
                _count = 0;
            }
        }

        private void ReleaseAllClips()
        {
            _soundPlayer.ReleaseAllClips();
        }
        
        private GameObject GetAudioSource()
        {
            var source = _objects.FirstOrDefault();
            if (source is null)
            {
                source = CreateNewSource();
            }

            _objects.Remove(source);
            _usingSources.Add(source);
            return source;
        }

        private GameObject CreateNewSource()
        {
            var audioSource = new GameObject($"AudioSource", typeof(AudioSource));
            audioSource.transform.SetParent(transform);
            _objects.Add(audioSource.gameObject);
            return audioSource.gameObject;
        }
        
        private void OnAudioSourceStopped(GameObject go)
        {
            _usingSources.Remove(go);
            _objects.Add(go);
        }


        private void OnDestroy()
        {
            if(_playButton!= null)
                _playButton.onClick.RemoveAllListeners();
            if(_playInterrupted != null)
                _playInterrupted.onClick.RemoveAllListeners();
            if(_playAt != null)
                _playAt.onClick.RemoveAllListeners();
            if(_releaseAllClips != null)
                _releaseAllClips.onClick.RemoveAllListeners();
            if (_playLoopedButton != null) 
                _playLoopedButton.onClick.RemoveAllListeners();  
            if(_pauseLoopedButton != null)
                _pauseLoopedButton.onClick.RemoveAllListeners();
            if (_resumeLoopedButton != null)
                _resumeLoopedButton.onClick.RemoveAllListeners();
            if(_stopLoopedButton != null)
                _stopLoopedButton.onClick.RemoveAllListeners();

        }
    }
}