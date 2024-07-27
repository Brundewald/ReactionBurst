using System.Collections.Generic;
using System.Threading;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using MyProject.Config;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.Core.Subflows;
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.SaveLoad;
using MyProject.ReactionBurst.Shared.SceneData;
using MyProject.ReactionBurst.Shared.ScenesProvider;
using MyProject.ReactionBurst.UI;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace MyProject.ReactionBurst.Core
{
    public sealed class StateMachine : MonoBehaviour
    {
        private NavigationPanelState _navigationState;
        private StartupFlow _startupFlow;
        private GameplayState _gameplayState;
        private EndGameState _endGameState;
        private PauseMenuState _pauseState;
        
        private IUIService _uiService;
        private IAudioService _audioService;
        private ISaveLoadService _savedDataService;
        private IConfigsProvider _configsProvider;
        private SceneProvider _sceneProvider;
        
        private CancellationTokenSource _cts;
        private bool _gameRestart;

        ///private bool isTutorialCompleted => _savedDataProvider.GetData<bool>(SaveDataNameConstants.IsTutorialCompleted);

        [Inject]
        public void Construct(
            IUIService uiService,
            ISaveLoadService  saveLoadService, 
            IConfigsProvider configsProvider, 
            IAudioService audioService)
        {
            _uiService = uiService;
            _audioService = audioService;
            _configsProvider = configsProvider;
            _savedDataService = saveLoadService;
        }

        private async void Start()
        {
            await Preload();
            OnLoaded();
        }

        private async UniTask Preload()
        {
            var gameData = await _configsProvider.GetConfigAsync<GameConfig>();
            Assert.IsNotNull(gameData);
            
            await _audioService.Preload();
            
            _navigationState = new NavigationPanelState(_uiService, _audioService);
            _pauseState = new PauseMenuState(_uiService, _audioService);
            _startupFlow = new StartupFlow(_uiService, _audioService, gameData);
            _gameplayState = new GameplayState(_uiService, gameData, _audioService);
            _endGameState = new EndGameState(_uiService, _audioService, _savedDataService);

            SetDataForCollection();
        }

        private void OnLoaded() => StartFlowsAsync();

        private async void StartFlowsAsync()
        {
            _gameRestart = false;
            _audioService.PlayMusic(AudioNameConstants.MainTheme);
            _cts = new CancellationTokenSource();
            
            await _navigationState.StartFlowAsync();
            
            SubscribeToEvents();

            //if (!isTutorialCompleted)
            //    await RunTutorialSequence();
            
            var canceled = await _startupFlow.StartAsync(_cts.Token);

            var gameResult = 0;
            
            if(!canceled)
                (canceled, gameResult) = await _gameplayState.StartAsync(_cts.Token);

            if (!canceled)
            {
                SetDataForCollection();
                _gameRestart = await _endGameState.StartAsync(gameResult);
            }
            HandleEndGame();
        }

        private void HandleEndGame()
        {
            if (_gameRestart)
                StartFlowsAsync();
            else
                EndFlow();
        }

        private void EndFlow()
        {
            //Освобождаем ресурсы и останавливаем процессы игры
            _audioService.Release();
            //TweenPauseTokenHolder.ClearInstance();
            //Если запущено из коллекции, то возвращаемся в нее
            if(_sceneProvider != null && _sceneProvider.HasSceneData)
            {
                FindObjectOfType<SceneProvider>().LoadScene("Collection");
            }
        }
        
        private async UniTask RunTutorialSequence()
        {
            //_sharedUIDirector.TutorialPlayButtonPressed += EndTutorial;
            //_sharedUIDirector.ShowTutorialView(false);
            //await UniTask.WaitUntil(() => isTutorialCompleted);
        }

        private void EndTutorial()
        {
            //_sharedUIDirector.TutorialPlayButtonPressed -= EndTutorial;
            //_sharedUIDirector.HideTutorialView(false);
            //_savedDataProvider.SetData(true, SaveDataNameConstants.IsTutorialCompleted);
        }
        
        private void OnGamePaused()
        {
            LitMotionControlExtension.PauseMotion();
            _startupFlow.Pause();
            _gameplayState.Pause();
        }

        private void OnGameUnpaused(bool gameSkipped)
        {
            if(!gameSkipped)
            {
                LitMotionControlExtension.UnpauseMotion();
                _startupFlow.Continue();
                _gameplayState.Continue();
            }
            else
            {
                _gameRestart = false;
                Cancel();
            }
        }

        private void OnGameRestarted()
        {
            _gameRestart = true;
            LitMotionControlExtension.UnpauseMotion();
            Cancel();
        }

        private void Cancel()
        {
            LitMotionControlExtension.CancelMotion();
            CancelFlows();
            _cts.Cancel();
        }

        private void OnSoundToggled(bool state) => _audioService.SetSoundsEnabled(state);

        private void OnMusicToggled(bool state) => _audioService.SetMusicEnabled(state);

        private void CancelFlows()
        {
            UnsubscribeToEvents();
            _startupFlow.Cancel();
            _gameplayState.Cancel();
            _navigationState.Cancel();
        }

        private void SetDataForCollection()
        {
            if (_sceneProvider != null)
            {
                _sceneProvider.SetSceneData(new GameSceneData()
                {
                    GameId = NameConstants.GameId,
                    //IsComplete = _savedDataProvider.GetData<int>("CurrentScore") > 0
                });
            }
        }

        private void SubscribeToEvents()
        {
            _navigationState.GamePaused += OnGamePaused;
            _navigationState.GameUnpaused += OnGameUnpaused;
            _navigationState.GameRestarted += OnGameRestarted;
            _navigationState.SoundToggled += OnSoundToggled;
            _navigationState.MusicToggled += OnMusicToggled;
        }

        private void UnsubscribeToEvents()
        {
            _navigationState.GamePaused -= OnGamePaused;
            _navigationState.GameUnpaused -= OnGameUnpaused;
            _navigationState.GameRestarted -= OnGameRestarted;
            _navigationState.SoundToggled -= OnSoundToggled;
            _navigationState.MusicToggled -= OnMusicToggled;
        }

#if !DISABLE_SRDEBUGGER

        private void CheatsOnTestCheatUsed()
        {
            Debug.Log("ExampleGame Test cheat log");
        }

#endif
    }
}