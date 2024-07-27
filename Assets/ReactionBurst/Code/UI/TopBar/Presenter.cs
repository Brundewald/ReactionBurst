using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.TopBar
{
    public sealed class Presenter : BasePresenter
    {
        private readonly Model _model;
        private readonly TopBarView _topBarView;

        public Presenter(TopBarView view, Model data) : base(view, data)
        {
            _model = data;
            _topBarView = view;
        }

        public void UpdateTimer(int time)
        {
            _model.TimerLeft = time;
        }

        public void UpdateScore(int score)
        {
            _model.Score = score;
        }

        public void SetAddScoreValue(int addScoreValue)
        {
            _model.AddScoreValue = addScoreValue;
        }
    }
}