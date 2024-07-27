using System;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace MyProject.ReactionBurst.UI.Visualization.Pause
{
    public class Visualization : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private CanvasGroup _pauseCanvasGroup;
        [SerializeField] private CanvasGroup _settingsCanvasGroup;
        [SerializeField] private Settings _settings;

        private void OnEnable()
        {
            _settingsCanvasGroup.alpha = 0f;
            _settingsCanvasGroup.gameObject.SetActive(false);
            _pauseCanvasGroup.gameObject.SetActive(true);
        }
        
        public async UniTask PlayOpenInitializationAsync()
        {
            await LMotion
                .Create(0f, 1f, -_settings.FadeInTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_mainCanvasGroup)
                .AddTo(this);
        }

        public async UniTask PlayCloseInitializationAsync()
        {
            await LMotion
                .Create(1f, 0f, _settings.FadeOutTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_mainCanvasGroup)
                .AddTo(this);
        }

        public async UniTask SwitchToSettingsAsync()
        {
            await LMotion
                .Create(1f, 0f, _settings.FadeOutTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_pauseCanvasGroup)
                .AddTo(this);
            
            _pauseCanvasGroup.gameObject.SetActive(false);
            _settingsCanvasGroup.gameObject.SetActive(true);

            await LMotion
                .Create(0f, 1f, _settings.FadeInTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_settingsCanvasGroup)
                .AddTo(this);
        }
        
        public async UniTask SwitchToPauseAsync()
        {
            await LMotion
                .Create(1f, 0f, _settings.FadeOutTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_settingsCanvasGroup)
                .AddTo(this);
            
            _settingsCanvasGroup.gameObject.SetActive(false);
            _pauseCanvasGroup.gameObject.SetActive(true);
            
            await LMotion
                .Create(0f, 1f, _settings.FadeInTime)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_pauseCanvasGroup)
                .AddTo(this);
        }
    }

    [Serializable]
    internal class Settings
    {
        public float FadeInTime;
        public float FadeOutTime;
        public AnimationCurve AnimationCurve;
    }
}