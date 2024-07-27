using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace MyProject.ReactionBurst.UI.EndGame
{
    public class Visualization : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private CanvasGroup _resultCanvasGroup;
        [SerializeField] private CanvasGroup _leaderboardCanvasGroup;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _leaderboardPositionText;
        
        [SerializeField] private Settings _settings;
        
        
        public async UniTask PlayOpenVisualizationAsync()
        {
            await CanvasAlphaAnimationAsync(_mainCanvasGroup,0f, 1f, _settings.OpenVisualizationDuration, _settings.Ease);
        }

        public async UniTask PlayCloseVisualizationAsync()
        {
            await CanvasAlphaAnimationAsync(_mainCanvasGroup, 1f, 0f, _settings.CloseVisualizationDuration, _settings.Ease);
        }

        public async UniTask PlaySwitchToLeaderBoardAsync()
        {
            await CanvasAlphaAnimationAsync(_resultCanvasGroup,1f, 0f, _settings.CloseVisualizationDuration, _settings.Ease);
            _resultCanvasGroup.gameObject.SetActive(false);
            _leaderboardCanvasGroup.gameObject.SetActive(true);
            await CanvasAlphaAnimationAsync(_leaderboardCanvasGroup,0f, 1f, _settings.CloseVisualizationDuration, _settings.Ease);
        }
        
        public async UniTask PlaySwitchToResultAsync()
        {
            await CanvasAlphaAnimationAsync(_leaderboardCanvasGroup,1f, 0f, _settings.CloseVisualizationDuration, _settings.Ease);
            _leaderboardCanvasGroup.gameObject.SetActive(false);
            _resultCanvasGroup.gameObject.SetActive(true);
            await CanvasAlphaAnimationAsync(_resultCanvasGroup,0f, 1f, _settings.CloseVisualizationDuration, _settings.Ease);
        }

        private async UniTask CanvasAlphaAnimationAsync(CanvasGroup canvasGroup, float from, float to, float duration, Ease ease)
        {
            await LMotion.Create(from, to, duration)
                .WithEase(ease)
                .BindToCanvasGroupAlpha(canvasGroup)
                .AddTo(this);
        }
    }

    [Serializable]
    internal class Settings
    {
        [field: SerializeField] public float OpenVisualizationDuration { get; private set; } = 0.5f;

        [field: SerializeField] public float CloseVisualizationDuration { get; private set;} = 0.5f;
        [field: SerializeField] public Ease Ease { get; private set; } = Ease.InSine;
    }
}