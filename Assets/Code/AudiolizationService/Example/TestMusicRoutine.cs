using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public class TestMusicRoutine: MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<string> _keys;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _playQueuedButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private bool _loadFromResources;
        private MusicPlayer _musicPlayer;
        
        
        private void Awake()
        {
            IAudioClipLoader audioClipLoader;
            if (_loadFromResources)
                audioClipLoader = new ResourcesAudioClipLoader();
            else
                audioClipLoader = new AddressableAudioClipLoader();
            
            _musicPlayer = new MusicPlayer(audioClipLoader);
            _musicPlayer.Initialize(_audioMixerGroup, _audioSource);
            _musicPlayer.AllClipsReleased += ReloadClips;
            _musicPlayer.Preload(_keys).Forget();
            if(_playButton != null)
                _playButton.onClick.AddListener(PlayClip);
            if(_playQueuedButton != null)
                _playQueuedButton.onClick.AddListener(PlayQueued);
            if(_nextButton != null)
                _nextButton.onClick.AddListener(NextClip);
            if(_stopButton != null)
                _stopButton.onClick.AddListener(StopPlaying);
        }
        
        private void PlayClip()
        {
            _musicPlayer.PlayNow(_keys.First());
        }

        private void PlayQueued()
        {
            _musicPlayer.PlayQueued();
        }

        private void NextClip()
        {
            _musicPlayer.NextClip(false);
        }

        private void StopPlaying()
        {
            _musicPlayer.Stop();
        }

        private void ReloadClips()
        {
            _musicPlayer.Preload(_keys).Forget();
        }


        private void OnDestroy()
        {
            if(_playButton != null)
                _playButton.onClick.RemoveAllListeners();
            
            if(_playQueuedButton != null)
                _playQueuedButton.onClick.RemoveAllListeners();
            
            if(_nextButton != null)
                _nextButton.onClick.RemoveAllListeners();
            
            if(_stopButton != null)
                _stopButton.onClick.RemoveAllListeners();
        }
    }
}