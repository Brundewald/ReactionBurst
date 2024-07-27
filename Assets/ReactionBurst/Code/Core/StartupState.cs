using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.Config;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI.Countdown;
using MyProject.ReactionBurst.UI;
using Timer = MyProject.ReactionBurst.Core.SubSystems.Timer;

namespace MyProject.ReactionBurst.Core
{
    public sealed class StartupState
    {
        private readonly IUIService _service;
        private readonly IAudioService _audioService;
        private readonly Timer _timer;
        private readonly int _waitSecondsBeforeStart;
        private Presenter _presenter;

        private IConfigsProvider _configsProvider;
        private GameConfig _gameConfig;


        public StartupState(IUIService service, IAudioService audioService, GameConfig gameConfig)
        {
            _waitSecondsBeforeStart = gameConfig.SecondsBeforeStart;
            _service = service;
            _audioService = audioService;
            _timer = new Timer();
        }

        public async UniTask<bool> StartAsync(CancellationToken token)
        {
            await PrepareAsync();
            await StartCountDownAsync(token).SuppressCancellationThrow();
            await ExitState();
            return token.IsCancellationRequested;
        }

        public void Pause() => _timer.Pause();

        public void Continue() => _timer.Continue();

        public void Cancel() => _timer.StopTimer();

        private async UniTask PrepareAsync()
        {
            _presenter = await _service
                .ConstructWindowAsync<Presenter>(NameConstants.GameStartView);
        }

        private async UniTask StartCountDownAsync(CancellationToken token)
        {
            var presenter = _service.ShowWindow<Presenter>();

            await presenter.PlayOpenVisualizationAsync();
            await UniTask.Delay(250, cancellationToken: token);
            
            _timer.TimeChanged += TimerTick;

            _timer.RunTimer(_waitSecondsBeforeStart);

            await _timer.WaitForElapse();
        }

        private async UniTask ExitState()
        {
            _service.HideWindow<Presenter>();
            _timer.TimeChanged -= TimerTick;
        }

        private void TimerTick(int timeLeft)
        {
            //_audioService.PlaySound(SharedSFXNames.Timer).Forget();
            _presenter.UpdateCountDown(timeLeft);
        }
    }
}
