using System;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.Results
{
    public class EndGamePresenter : BasePresenter
    {
        public event Action ExitToMenu = delegate { };
        public event Action Replay = delegate { };
        
        private readonly EndGameModel _endGameModel;
        private readonly EndGameView _endGameView;
        
        public EndGamePresenter(EndGameView endGameView, EndGameModel data) : base(endGameView, data)
        {
            _endGameModel = data;
            _endGameView = endGameView;
            SubscribeToViewEvents(endGameView);
        }
        
        private void SubscribeToViewEvents(EndGameView endGameView)
        {
            endGameView.PlayNextGame += ViewOnPlayNextGame;
            endGameView.Replay += ViewOnReplay;
        }

        private void ViewOnReplay()
        {
            Replay();
        }

        private void ViewOnPlayNextGame()
        {
            ExitToMenu();
        }
    }
}