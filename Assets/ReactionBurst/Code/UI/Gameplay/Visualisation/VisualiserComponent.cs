using System;
using System.Threading;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MyProject.ReactionBurst.Constants;
using UnityEngine;

namespace MyProject.ReactionBurst.UI.Gameplay.Visualisation
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + nameof(VisualiserComponent))]
    public class VisualiserComponent: MonoBehaviour
    {
        [SerializeField] private RectTransform _scrollContent;
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private CanvasGroup _buttonCanvasGroup;
        [SerializeField] private CanvasGroup _notationCanvasGroup;
        [SerializeField] private Settings _settings;
        
        private CancellationToken _token;
        private CompositeMotionHandle _handles;
        private CompositeMotionHandle Handles => _handles ??= new CompositeMotionHandle();

        public event Action ScrollAnimationCompleted = delegate {  };

        private void Awake()
        {
            _token = gameObject.GetCancellationTokenOnDestroy();
        }

        public async UniTask PlayShowAnimationAsync()
        {
            var handle = LMotion
                .Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_mainCanvasGroup)
                .AddTo(Handles)
                .RegisterActiveHandle();

            await handle;
            
            Handles.Remove(handle);
        }

        public async UniTask ShowNotationAsync()
        {
            var handle = LMotion
                .Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_notationCanvasGroup)
                .AddTo(Handles)
                .RegisterActiveHandle();
            
            await handle;
            
            Handles.Remove(handle);
        }

        public async UniTask HideNotationAsync()
        {
            var handle = LMotion
                .Create(1f, 0f, _settings.FadeOutDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_notationCanvasGroup)
                .AddTo(Handles)
                .RegisterActiveHandle();
            await handle;
            Handles.Remove(handle);
        }

        public async UniTask ShowButtonsAnimationAsync()
        {
            var handle = LMotion
                .Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_buttonCanvasGroup)
                .AddTo(Handles)
                .RegisterActiveHandle();

            await handle;
            Handles.Remove(handle);
        }

        public async UniTask PlayScrollAnimationAsync(Vector3 offset)
        {
            var position = _scrollContent.localPosition - offset;

            var handle = LMotion
                .Create(_scrollContent.localPosition.x, position.x, _settings.ScrollAnimationDuration)
                .WithEase(_settings.ScrollAnimationCurve)
                .BindToLocalPositionX(_scrollContent)
                .AddTo(Handles)
                .RegisterActiveHandle();
            
            await handle;
            
            Handles.Remove(handle);
            
            ScrollAnimationCompleted.Invoke();
        }

        public void ResetView()
        {
            Handles.Complete();
            Handles.Clear();
            _mainCanvasGroup.alpha = 0f;
            _buttonCanvasGroup.alpha = 0f;
            _notationCanvasGroup.alpha = 0f;
            _scrollContent.anchoredPosition = Vector2.zero;
        }

        [Serializable]
        private class Settings
        {
            [field: SerializeField] public float FadeInDuration { get; private set; } = .1f;
            [field: SerializeField] public float FadeOutDuration { get; private set; } = .1f;
            [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; } = AnimationCurve.Linear(0,0,1,1);
            [field: SerializeField] public float ScrollAnimationDuration { get; private set; } = .15f;
            [field: SerializeField] public AnimationCurve ScrollAnimationCurve { get; private set; } = AnimationCurve.Linear(0,0,1,1);
        }
    }
}