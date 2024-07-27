using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.UI;
using Shared_Code.SharedConstants;

namespace MyProject.ReactionBurst.Core.Subflows
{
    public sealed class NavigationPanelState
    {
        private readonly PauseMenuState _pauseMenuState;
        private readonly IUIService _uiService;
        
        private bool _gameSkipped;
        private bool _infoButtonPressed;
        private UI.NavigationPanel.Presenter _navigationPanel;
        private UI.Pause.Presenter _pauseMenu;

        public event Action<bool> SoundToggled = delegate {  };
        public event Action<bool> MusicToggled = delegate {  };
        public event Action GamePaused = delegate {};
        public event Action<bool> GameUnpaused = delegate { };
        public event Action GameRestarted = delegate {  };

        public NavigationPanelState(IUIService uiService, IAudioService audioService)
        {
            _uiService = uiService;
            _pauseMenuState = new PauseMenuState(uiService, audioService);
        }

        public async UniTask StartFlowAsync()
        {
            await PrepareAsync();
            await RunInBackGroundAsync();
        }

        private async UniTask PrepareAsync()
        {
            _navigationPanel = await _uiService.ConstructWindowAsync<UI.NavigationPanel.Presenter>("NavigationPanel");
            _pauseMenu = await _uiService.ConstructWindowAsync<UI.Pause.Presenter>("PauseMenuView");;
        }

        public void Cancel()
        {
            _pauseMenuState.Cancel();
            _uiService.HideWindow<UI.NavigationPanel.Presenter>();
            UnsubscribeToEvents();
        }

        private async UniTask RunInBackGroundAsync()
        {
            SubscribeToEvents();
            _uiService.ShowWindow<UI.NavigationPanel.Presenter>();
        }

        private void OnSoundStateChanged(bool state)
        {
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            SoundToggled.Invoke(state);
        }

        private void OnMusicStateChanged(bool state)
        {
            MusicToggled.Invoke(state);
        }

        private async void OnPauseButtonPressed()
        {
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            GamePaused.Invoke();

            bool canceled;
            
            (canceled, _gameSkipped) = await _pauseMenuState.StartFlowAsync().SuppressCancellationThrow();
            
            if(!canceled) UnpauseGame();
        }

        private void OnInfoButtonPressed()
        {
            if(_pauseMenuState.isActive) 
                return;

            _infoButtonPressed = true;
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            GamePaused.Invoke();
            //_sharedDirector.TutorialPlayButtonPressed += UnpauseGame;
            //_sharedDirector.ShowTutorialView(false);
        }

        private void UnpauseGame()
        {
            if(_infoButtonPressed)
            {
                //_sharedDirector.OpenNavigationPanelView();
                _infoButtonPressed = false;
                //_sharedDirector.HideTutorialView();
                //_sharedDirector.TutorialPlayButtonPressed -= UnpauseGame;
            }
            
            GameUnpaused.Invoke(_gameSkipped);
        }

        private void OnRestartButtonPressed()
        {
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            GameRestarted.Invoke();
            _pauseMenuState.Cancel();
        }

        private void SubscribeToEvents()
        {
            _navigationPanel.PauseButtonPressed += OnPauseButtonPressed;
            //_sharedDirector.InfoButtonPressed += OnInfoButtonPressed;
            _pauseMenu.SFXStateChanged += OnSoundStateChanged;
            _pauseMenu.MusicStateChanged += OnMusicStateChanged;
            _pauseMenu.RestartButtonPressed += OnRestartButtonPressed;
        }

        private void UnsubscribeToEvents()
        {
            _navigationPanel.PauseButtonPressed -= OnPauseButtonPressed;
            //_sharedDirector.InfoButtonPressed += OnInfoButtonPressed;
            _pauseMenu.SFXStateChanged -= OnSoundStateChanged;
            _pauseMenu.MusicStateChanged -= OnMusicStateChanged;
            _pauseMenu.RestartButtonPressed -= OnRestartButtonPressed;
        }
    }
}