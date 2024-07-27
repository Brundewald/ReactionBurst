using System;

namespace MyProject.ReactionBurst.UI.NavigationPanel
{
    public class Presenter : BasePresenter
    {
        public event Action PauseButtonPressed = delegate { };
        public event Action InfoButtonPressed = delegate { };
        
        private readonly Model _model;
        private readonly View _view;
        
        public Presenter(View view, Model data) : base(view, data)
        {
            _view = view;
            _model = data;
            SubscribeToViewEvents(_view);
        }

        private void SubscribeToViewEvents(View view)
        {
            view.PauseButtonPressed += OnPauseButtonPressed;
            view.InfoButtonPressed += OnInfoButtonPressed;
        }

        private void OnPauseButtonPressed() => PauseButtonPressed();
        
        private void OnInfoButtonPressed() => InfoButtonPressed();

        public void EnableButtons(bool isEnabled) => _view.EnableButtons(isEnabled);

        public void CloseView() => _model.NeedClose = true;
    }
}