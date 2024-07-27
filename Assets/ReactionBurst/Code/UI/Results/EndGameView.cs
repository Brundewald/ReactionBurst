using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Results
{
    [AddComponentMenu("Games/GameCollection/UI/ResultsView")]
    public class EndGameView : BaseView
    {
        public event Action PlayNextGame = delegate { };

        public event Action Replay = delegate { };

        [SerializeField]
        private TextMeshProUGUI _currentScoreTypeText;

        [SerializeField]
        private TextMeshProUGUI _currentScoreInfoText;

        [SerializeField]
        private TextMeshProUGUI _bestScoreText;

        [SerializeField]
        private RectTransform _benefitsRoot;

        [SerializeField]
        private CharacteristicView _characteristicView;

        [SerializeField]
        private Image _currentProgress;

        [SerializeField]
        private RectTransform _starMark;

        [SerializeField]
        private RectTransform _markContainer;

        [SerializeField]
        private VerticalLayoutGroup _contentGroup;
        
        private int _currentScore;

        public override void Prepare<T>(T data)
        {
            var model = data as EndGameModel;
            Assert.IsNotNull(model);

            _currentScore = model.CurrentScore;
            _bestScoreText.text = model.BestScore.ToString();

            _currentProgress.fillAmount = (float) model.CurrentScore / model.MaximumScore;

            ShowStarMark(model.CurrentScore, model.StarMarkScore);
        }

        private void OnEnable()
        {
            //_currentScoreText.ChangeTo(_currentScore);
            _contentGroup.enabled = true;
            HackForLayout().Forget();
        }
        
        private async UniTask HackForLayout()
        {
            await UniTask.WaitForEndOfFrame(this);
            _contentGroup.enabled = false;
            await UniTask.WaitForEndOfFrame(this);
            _contentGroup.enabled = true;
        }
        
        private void ShowStarMark(int currentScore, int starMarkScore)
        {
            const float startAngle = 210; // note: эмпирически выбранный стартовый угол
            const float maxOffsetAngle = 240; // note: максимальный угол по концепту

            float starMarkProgress = (float) currentScore / starMarkScore;

            starMarkProgress = Mathf.Clamp01(starMarkProgress);

            float finalAngle = startAngle - starMarkProgress * maxOffsetAngle;

            float diameter = _markContainer.sizeDelta.x; // note: ширина Rect подобрана под концепт

            ShowStarMarkVisualization(startAngle, finalAngle, diameter, _markContainer.rect.center).Forget();
        }

        private async UniTask ShowStarMarkVisualization(float startAngle, float finalAngle, float diameter,
            Vector2 center)
        {
            const float duration = 3f;
            const int steps = 20;

            float rotationStep = (finalAngle - startAngle) / steps;

            var startPosition = GetPositionOnCircle(diameter, center, startAngle);

            //await _starMark.DOAnchorPos(startPosition, 0);

            for (int i = 1; i <= steps; i++)
            {
                float angle = startAngle + rotationStep * i;

                Vector2 position = GetPositionOnCircle(diameter, center, angle);

                //await _starMark.DOAnchorPos(position, duration / steps).SetEase(Ease.Linear);
            }
        }

        private Vector2 GetPositionOnCircle(float diameter, Vector2 center, float angle)
        {
            float angleRadians = Mathf.Deg2Rad * angle;

            float radius = diameter / 2f;

            float x = center.x + radius * Mathf.Cos(angleRadians);
            float y = center.y + radius * Mathf.Sin(angleRadians);

            return new Vector2(x, y);
        }

        public void OnPlayNextGame()
        {
            OnCloseButtonPressed();
            PlayNextGame();
        }

        public void OnReplay()
        {
            OnCloseButtonPressed();
            Replay();
        }
    }
}