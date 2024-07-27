using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.Gameplay
{
    public sealed class Presenter : BasePresenter, IDisposable
    {
        private readonly View _view;
        private readonly Model _model;
        private int _answerCount;

        public event Action<bool, ComparableFigure> AnswerButtonClicked = delegate { };

        public Presenter(View view, Model model) : base(view, model)
        {
            _view = view;
            _model = model;
            SubscribeToViewEvents();
        }

        public void Dispose() => 
            UnsubscribeFromEvents();

        public async UniTask PlayAnswerSequenceAsync(bool correctAnswer) => 
            await _view.ShowAnswerAsync(correctAnswer);

        public async UniTask UpdateScrollPositionAsync() => 
            await _view.UpdateScrollPositionAsync();

        public void KillAnimations() => 
            _view.KillAnimations();

        private void OnAnswerButtonClicked(bool sameFigure, ComparableFigure figure) => 
            AnswerButtonClicked.Invoke(sameFigure, figure);

        private void SubscribeToViewEvents() => 
            _view.AnswerButtonClicked += OnAnswerButtonClicked;

        private void UnsubscribeFromEvents() => 
            _view.AnswerButtonClicked -= OnAnswerButtonClicked;

        public void SetScoreModifier(int modifier) => _model.ScoreModifier = modifier;

        public void SetInitialPool(ComparableFigure[] initialFiguresPool) => _model.InitialFiguresPool = initialFiguresPool;
        
        public void SetFigureAsync(ComparableFigure figure) => _model.Figure = figure;
    }
}