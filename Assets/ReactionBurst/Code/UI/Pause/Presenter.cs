using System;

namespace MyProject.ReactionBurst.UI.Pause
{
    public sealed class Presenter : BasePresenter
    {
        public event Action ContinueButtonPressed = delegate { };
        public event Action RestartButtonPressed = delegate { };
        public event Action InstructionButtonPressed = delegate { };
        public event Action ExitButtonPressed = delegate { };
        public event Action<bool> SFXStateChanged = delegate { };
        public event Action<bool> MusicStateChanged = delegate { };

        private readonly Model _model;
        private readonly View _view;
        
        public Presenter(View view, Model data) : base(view, data)
        {
            _view = view;
            _model = data;
            SubscribeToViewEvents(view);
        }

        public void SetSFXState(bool state) => 
            _model.SFXEnabled = state;

        public void SetMusicState(bool state) => 
            _model.MusicEnabled = state;

        private void SubscribeToViewEvents(View view)
        {
            view.ContinueButtonPressed += OnContinueButtonPressed;
            view.RestartButtonPressed += OnRestartButtonPressed;
            view.InstructionButtonPressed += OnInstructionButtonPressed;
            view.ExitButtonPressed += OnExitButtonPressed;
            view.SFXToggled += OnSFXToggled;
            view.MusicToggled += OnMusicToggled;
        }

        private void OnContinueButtonPressed() => ContinueButtonPressed();
        private void OnRestartButtonPressed() => RestartButtonPressed();
        private void OnInstructionButtonPressed() => InstructionButtonPressed();
        private void OnExitButtonPressed() => ExitButtonPressed();
        private void OnSFXToggled(bool state) => SFXStateChanged(state);
        private void OnMusicToggled(bool state) => MusicStateChanged(state);
    }
}