using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.Games.RecallRun.EndGame
{
    public class Presenter : BasePresenter
    {
        private Model _model;
        private View _view;
        
        protected Presenter(View view, Model gameplayModel) : base(view, gameplayModel)
        {
            
        }
    }
}