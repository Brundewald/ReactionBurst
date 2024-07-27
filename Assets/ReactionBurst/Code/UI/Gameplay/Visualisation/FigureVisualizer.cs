using System;
using System.Threading;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MyProject.ReactionBurst.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Gameplay.Visualisation
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + nameof(FigureVisualizer))]
    public sealed class FigureVisualizer: MonoBehaviour
    {
        private const string WrongEmoji = "<sprite name=Wrong>";
        private const string CorrectEmoji = "<sprite name=Correct>";
        
        [SerializeField] private Sprite _correctBackground;
        [SerializeField] private Sprite _wrongBackground;
        [SerializeField] private Image _answerImage;
        [SerializeField] private TextMeshProUGUI _answerTextField;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Settings _settings;
        
        private CancellationToken _token;
        private MotionHandle _handle;

        private void Awake() => 
            _token = gameObject.GetCancellationTokenOnDestroy();

        private void OnDisable() => 
            ResetView();

        public async UniTask ShowAnswerResult(bool correctAnswer, string multiplier = "")
        {
            if (correctAnswer)
                _answerTextField.text = multiplier is "" ? CorrectEmoji : $"x{multiplier}";
            else
                _answerTextField.text = multiplier is "" ? WrongEmoji : $"x{multiplier}";

            _answerImage.sprite = correctAnswer ? _correctBackground : _wrongBackground;

            LMotion.Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_canvasGroup)
                .AddTo(this)
                .RegisterActiveHandle();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_settings.ScrollDelay), cancellationToken: _token);
        }

        public void ResetView()
        {
            _canvasGroup.alpha = 0;
            _answerTextField.text = "";
        }

        [Serializable]
        private class Settings
        {
            [field: SerializeField] public float FadeInDuration { get; private set; }
            [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; }
            [field: SerializeField] public float ScrollDelay { get; private set; } = 0.2f;
        }
    }
}