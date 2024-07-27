using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.Countdown
{
    public sealed class Presenter : BasePresenter
    {
        private readonly View _view;

        public Presenter(View view, Model data) : base(view, data)
        {
            _view = view;
        }

        public async UniTask PlayOpenVisualizationAsync() => 
            await _view.PlayOpenVisualisationAsync();

        public void UpdateCountDown(int timeLeft)
        {
            _view.OnCountdownChanged(timeLeft);
        }
    }
}