using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Data;

namespace MyProject.ReactionBurst.UI.EndGame
{
    public class Presenter : BasePresenter
    {
        private readonly Model _model;
        private readonly View _view;

        public event Action ExitButtonPressed = delegate { };
        public event Action RetryButtonPressed = delegate { };
        
        public Presenter(View view, Model model) : base(view, model)
        {
            _model = model;
            _view = view;
            _view.ExitButtonPressed += HandleExitButtonPressed;
            _view.RetryButtonPressed += HandleRetryButtonPressed;
        }

        public override void Dispose()
        {
            base.Dispose();
            _view.ExitButtonPressed -= HandleExitButtonPressed;
            _view.RetryButtonPressed -= HandleRetryButtonPressed;
        }

        public void UpdatePlayerResult(LeaderBoardData playerData, LeaderBoardData[] leaderBoardData)
        {
            _model.PlayerLeaderboardData = playerData;
            _model.LeaderboardData = leaderBoardData;
        }

        public async UniTask WarmupAsync(int leaderBoardSize) => await _view.WarmUpScrollAsync(leaderBoardSize);

        public async UniTask OpenAnimationAsync() => await _view.OpenAnimationAsync();

        public async UniTask CloseAnimationAsync() => await _view.CloseAnimationAsync();

        private void HandleExitButtonPressed() => ExitButtonPressed.Invoke();

        private void HandleRetryButtonPressed() => RetryButtonPressed.Invoke();
    }
}