using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.SaveLoad;
using MyProject.ReactionBurst.UI;
using Shared_Code.SharedConstants;
using Zenject;

namespace MyProject.ReactionBurst.Core
{
    public sealed class EndGameState : IInitializable
    {
        private readonly IAudioService _audioService;
        private readonly ISaveLoadService _savedDataService;
        private readonly IUIService _uiService;

        private bool _closed;
        private bool _restart;

        public EndGameState(IUIService uiService, IAudioService audioService, ISaveLoadService savedDataService)
        {
            _audioService = audioService;
            _uiService = uiService;
            _savedDataService = savedDataService;
        }

        public void Initialize()
        {
            //_exitPresenter = _uiDirector.GetWindow<EndGamePresenter>();
        }

        private void SubscribeToEvents()
        {
            //_sharedUiDirector.RestartButtonPressed += OnRestartButtonClicked;
            //_sharedUiDirector.ExitButtonPressed += OnViewHidden;
        }

        private void UnsubscribeFromEvents()
        {
            //_sharedUiDirector.RestartButtonPressed -= OnRestartButtonClicked;
            //_sharedUiDirector.ExitButtonPressed -= OnViewHidden;
        }

        public async UniTask<bool> StartAsync(int resultScore)
        {
            await PrepareAsync(resultScore);
            return await ShowEndGameAsync();
        }

        private async UniTask PrepareAsync(int resultScore)
        {
            //SubscribeToEvents();
            SaveProgress(resultScore);
            _closed = false;
            _restart = false;
        }

        private async UniTask<bool> ShowEndGameAsync()
        {
            //_sharedUiDirector.ShowResultsView();
            _audioService.PlaySound(AudioNameConstants.LevelCompleted).Forget();
            await UniTask.WaitUntil(() => _closed || _restart);
            //UnsubscribeFromEvents();
            return _restart;
        }

        private void OnViewHidden()
        {
            _audioService.PlaySound(SharedSFXNames.Click).Forget();
            _closed = true;
        }

        private void OnRestartButtonClicked()
        {
            _audioService.PlaySound(SharedSFXNames.Click).Forget();
            _restart = true;
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
