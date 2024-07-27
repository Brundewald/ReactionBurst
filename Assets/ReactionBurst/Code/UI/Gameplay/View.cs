using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI.Gameplay.SubView;
using MyProject.ReactionBurst.UI.Gameplay.Visualisation;
using MyProject.ReactionBurst.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Gameplay
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + NameConstants.GameplayView)]
    public sealed class View : BaseView
    {
        private readonly Vector3 _offset = new(Screen.width, 0, 0);

        [SerializeField] private TextMeshProUGUI _riddleTypeNotification;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private VisualiserComponent visualiser;
        [SerializeField] private float _delayBeforeFirstScroll = 1;
        [SerializeField] private LocalizedString _firstMessage;
        [SerializeField] private LocalizedString _secondMessage;
        [SerializeField] private List<FigureView> _scrollElements;

        private Vector3 _firstElementDefaultPosition;
        private string _scoreModifier;
        private bool _modifierUpdated;

        public event Action<bool, ComparableFigure> AnswerButtonClicked = delegate { };

        private void Awake()
        {
            var screenCenterPosition = new Vector3(Screen.width * .5f, 0, 0);
            _firstElementDefaultPosition = screenCenterPosition;
            _scrollElements.First().transform.localPosition = screenCenterPosition;
        }

        public override void Prepare<T>(T data)
        {
            var model = data as Model;
            Assert.IsNotNull(model);

            //_riddleTypeNotification.text = model.Notation.GetLocalizedString();
            
            OpenVisualizationAsync().Forget();
        }

        public void OnEnable() => 
            SubscribeButtons();

        public override void UpdateData(string dataName, object value)
        {
            if (dataName.Equals("Notation"))
            {
                var text = (LocalizedString) value;
                _riddleTypeNotification.text = text.GetLocalizedString();
            }
            
            if (dataName.Equals("InitialFiguresPool"))
            {
                PrepareFigures((ComparableFigure[]) value);
            }

            if (dataName.Equals("Figure"))
            {
                SetFigureField((ComparableFigure) value);
            }

            if (dataName.Equals("ScoreModifier"))
            {
                _scoreModifier = value.ToString();
                _modifierUpdated = (int) value > 1;
            }
        }

        public async UniTask ShowAnswerAsync(bool answerIsCorrect)
        {
            await _scrollElements.First().ShowAnswerResult(answerIsCorrect, _modifierUpdated ? _scoreModifier : "");
            if (_modifierUpdated) _modifierUpdated = false;
        }

        public async UniTask UpdateScrollPositionAsync()
        {
            await visualiser.PlayScrollAnimationAsync(_offset);
            ChangeButtonState(true);
        }

        private void OnDisable() => 
            UnsubscribeButtons();

        public void KillAnimations() => 
            visualiser.ResetView();

        private void PrepareFigures(ComparableFigure[] figures)
        {
            _scrollElements[0].transform.localPosition = _firstElementDefaultPosition;
            
            for (var i = 0; i < _scrollElements.Count; i++)
            {
                if (i > 0)
                    _scrollElements[i].transform.localPosition =
                        _scrollElements[i - 1].transform.localPosition + _offset;
                
                var element = _scrollElements[i];
                element.SetImage(figures[i].Sprite, figures[i].Color);
            }
        }

        private void SetFigureField(ComparableFigure figure)
        {
            var lastFigure = _scrollElements[^1];
            lastFigure.SetImage(figure.Sprite, figure.Color);
        }

        private async UniTask OpenVisualizationAsync()
        {
            ResetView();
            await visualiser.PlayShowAnimationAsync();
            
            //_riddleTypeNotification.text = _firstMessage;
            await visualiser.ShowNotationAsync();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFirstScroll));
            await visualiser.PlayScrollAnimationAsync(_offset);
            
            await visualiser.HideNotationAsync();
            
            //_riddleTypeNotification.text = _secondMessage;
            
            await visualiser.ShowNotationAsync();
            await visualiser.ShowButtonsAnimationAsync();
        }

        private void SubscribeButtons()
        {
            visualiser.ScrollAnimationCompleted += ResetFirstPosition;
            _yesButton.onClick.AddListener(OnYesButtonClicked);
            _noButton.onClick.AddListener(OnNoButtonClicked);
        }

        private void UnsubscribeButtons()
        {
            visualiser.ScrollAnimationCompleted -= ResetFirstPosition;
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }

        private void OnYesButtonClicked() => 
            HandleAnswerButton(true);

        private void OnNoButtonClicked() => 
            HandleAnswerButton(false);

        private async void HandleAnswerButton(bool answer)
        {
            ChangeButtonState(false);
            var firstElement = _scrollElements.First();
            var comparable = new ComparableFigure {Sprite = firstElement.FigureImage.sprite, Color = firstElement.FigureImage.color};
            AnswerButtonClicked.Invoke(answer, comparable);

            if (!_riddleTypeNotification.gameObject.activeSelf) return;
            
            await visualiser.HideNotationAsync();
            _riddleTypeNotification.gameObject.SetActive(false);
        }

        private void ChangeButtonState(bool state)
        {
            _yesButton.interactable = state;
            _noButton.interactable = state;
        }

        private void ResetFirstPosition()
        {
            var firstElement = _scrollElements.First();
            firstElement.transform.localPosition = _scrollElements[^1].transform.localPosition + _offset;
            _scrollElements.Remove(firstElement);
            _scrollElements.Add(firstElement);
            firstElement.ResetView();
        }

        private void ResetView()
        {
            visualiser.ResetView();
            _riddleTypeNotification.gameObject.SetActive(true);
        }
    }
}
