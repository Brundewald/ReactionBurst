using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.SaveLoad;
using MyProject.ReactionBurst.UI.Pause;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.Core.Subflows
{
    public sealed class PauseMenuState
    {
        private readonly IUIService _uiService;
        private readonly IAudioService _audioService;
        private CancellationTokenSource _cts;
        private bool _gameSkipped;
        private bool _isActive;

        public bool isActive => _isActive;

       public PauseMenuState(IUIService uiService, IAudioService audioService)
       {
           _uiService = uiService;
           _audioService = audioService;
       }
        
       public async UniTask<bool> StartAsync()
       {
           _cts = new CancellationTokenSource();
           var presenter = _uiService.GetWindow<Presenter>();
           presenter.SetMusicState(_audioService.MusicEnabled);
           presenter.SetSFXState(_audioService.SFXEnabled);
           return await PauseLoop().AttachExternalCancellation(_cts.Token);
       }

       private async UniTask<bool> PauseLoop()
       {
           _isActive = true;
           SubscribeToEvents();

           _uiService.ShowWindow<Presenter>();
           
           await UniTask.WaitUntil(() => !_isActive);
           
           _uiService.HideWindow<Presenter>();
           
           UnsubscribeFormEvents();

           return _gameSkipped;
       }

       private void SubscribeToEvents()
       {
           var pausePresenter = _uiService.GetWindow<Presenter>();
           pausePresenter.ContinueButtonPressed += OnContinueButtonPressed;
           pausePresenter.ExitButtonPressed += OnExitButtonPressed;
           pausePresenter.InstructionButtonPressed += OnInstructionButtonPressed;
           //pausePresenter.TutorialPlayButtonPressed += OnTutorialButtonPressed;
       }

       private void UnsubscribeFormEvents()
       {
           var pausePresenter = _uiService.GetWindow<Presenter>();
           pausePresenter.ContinueButtonPressed -= OnContinueButtonPressed;
           pausePresenter.ExitButtonPressed -= OnExitButtonPressed;
           pausePresenter.InstructionButtonPressed -= OnInstructionButtonPressed;
           //_sharedDirector.TutorialPlayButtonPressed -= OnTutorialButtonPressed;
       }

       private void OnInstructionButtonPressed()
       { 
           //_sharedDirector.ShowTutorialView(true);
       }

       private void OnTutorialButtonPressed()
       {
           //_sharedDirector.HideTutorialView();
           //_sharedDirector.ShowPauseView();
       }

       private void OnExitButtonPressed()
       {
           _gameSkipped = true;
           _isActive = false;
       }
       
       private void OnContinueButtonPressed()
       {
           _gameSkipped = false;
           _isActive = false;
       }
       
       public void Cancel()
       {
           _gameSkipped = false;
           _isActive = false;
           _cts?.Cancel();
       }
    }
}