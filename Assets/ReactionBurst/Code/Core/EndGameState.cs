using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.SaveLoad;
using MyProject.ReactionBurst.UI;
using MyProject.ReactionBurst.UI.EndGame;
using Shared_Code.SharedConstants;

namespace MyProject.ReactionBurst.Core
{
    public sealed class EndGameState
    {
        private readonly IAudioService _audioService;
        private readonly ISaveLoadService _savedDataService;
        private readonly IUIService _uiService;

        private bool _closed;
        private bool _restart;

        private Presenter _presenter;

        public EndGameState(IUIService uiService, IAudioService audioService, ISaveLoadService savedDataService)
        {
            _audioService = audioService;
            _uiService = uiService;
            _savedDataService = savedDataService;
        }

        public async UniTask PreloadAsync()
        {
            _presenter = await _uiService.ConstructWindowAsync<Presenter>("EndGameWindow");
            await _presenter.WarmupAsync(10);
        }

        public async UniTask<bool> StartAsync(int resultScore)
        {
            await PrepareAsync(resultScore);
            return await ShowEndGameAsync();
        }

        private async UniTask PrepareAsync(int resultScore)
        {
            SubscribeToEvents();
            SaveProgress(resultScore);
            _closed = false;
            _restart = false;
        }

        private async UniTask<bool> ShowEndGameAsync()
        {
            _presenter.Show();
            await _presenter.OpenAnimationAsync();
            //_audioService.PlaySound(AudioNameConstants.LevelCompleted).Forget();
            await UniTask.WaitUntil(() => _closed || _restart);
            UnsubscribeFromEvents();
            return _restart;
        }

        private void OnExitButtonPressed()
        {
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            _closed = true;
        }

        private void OnRestartButtonPressed()
        {
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();
            _restart = true;
        }

        private void SubscribeToEvents()
        {
            _presenter.ExitButtonPressed += OnRestartButtonPressed;
            _presenter.RetryButtonPressed += OnExitButtonPressed;
        }

        private void UnsubscribeFromEvents()
        {
            _presenter.ExitButtonPressed -= OnRestartButtonPressed;
            _presenter.RetryButtonPressed -= OnExitButtonPressed;
        }

        private void SaveProgress(int resultScore)
        {
            //var gameRounds = _savedDataProvider.GetData<int>(SaveDataNameConstants.GameRounds);
            //var totalScore = _savedDataProvider.GetData<int>(SaveDataNameConstants.TotalScore);
            //totalScore += resultScore;
            //gameRounds++;
            //
            //var average = totalScore / gameRounds;
            //
            //_savedDataProvider.SetData(gameRounds, SaveDataNameConstants.GameRounds);
            //_savedDataProvider.UpdateGameResult(resultScore, 0, UnityEngine.Random.Range(0, 100));
        }
    }
}
