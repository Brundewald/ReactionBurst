using System;
using System.Threading;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MyProject.ReactionBurst.Constants;
using UnityEngine;

namespace MyProject.ReactionBurst.UI.TopBar.Visualisation
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + nameof(TopBarVisualiser))]
    public sealed class TopBarVisualiser: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _dataCanvasGroup;
        [SerializeField] private CanvasGroup _additionalScoreField;
        [SerializeField] private AnimationSettings _settings;
        private Vector3 _defaultPosition;
        private CancellationToken _token;
        private CompositeMotionHandle _handles;
        private CompositeMotionHandle Handles => _handles ??= new CompositeMotionHandle();

        private void Awake()
        {
            _defaultPosition = _additionalScoreField.transform.localPosition;
            _token = gameObject.GetCancellationTokenOnDestroy();
        }

        public async UniTask PlayIntroAnimation()
        {
            var handle = LMotion
                .Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_dataCanvasGroup)
                .AddTo(Handles)
                .RegisterActiveHandle();

            await handle;
            Handles.Remove(handle);
        }

        public async UniTask PlayAddScoreAnimationAsync()
        {
            ResetAdditionalScoreField();

            var handleFadeIn = LMotion
                .Create(0f, 1f, _settings.FadeInDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_additionalScoreField)
                .AddTo(Handles)
                .RegisterActiveHandle();
            
            var handleMove = LMotion.Create(_additionalScoreField.transform.localPosition.y, _defaultPosition.y, _settings.FadeInDuration + _settings.FadeOutDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToLocalPositionY(_additionalScoreField.transform)
                .AddTo(Handles)
                .RegisterActiveHandle();

            await handleFadeIn;

            Handles.Remove(handleFadeIn);
            Handles.Remove(handleMove);
            
            var handleFadeOut = LMotion
                .Create(1f, 0f, _settings.FadeOutDuration)
                .WithEase(_settings.AnimationCurve)
                .BindToCanvasGroupAlpha(_additionalScoreField)
                .AddTo(Handles)
                .RegisterActiveHandle();
            
            await handleFadeOut;
            Handles.Remove(handleFadeOut);
        }

        private void ResetAdditionalScoreField()
        {
            _additionalScoreField.transform.localPosition = new Vector3(_defaultPosition.x, _defaultPosition.y - _settings.PositionOffset, _defaultPosition.z);
            _additionalScoreField.gameObject.SetActive(true);
        }

        public void ResetView()
        {
            Handles.Complete();
            Handles.Clear();
            
            _dataCanvasGroup.alpha = 0;
            _additionalScoreField.alpha = 0;
            _additionalScoreField.transform.localPosition = _defaultPosition;
            _additionalScoreField.gameObject.SetActive(false);
        }

        [Serializable]
        private class AnimationSettings
        {
            [field: SerializeField] public float PositionOffset { get; private set; } = 25;
            [field: SerializeField] public float FadeInDuration { get; private set; }= .2f;
            [field: SerializeField] public float FadeOutDuration { get; private set; }= .2f;
            [field: SerializeField] public AnimationCurve AnimationCurve { get; private set;} = AnimationCurve.Linear(0,0,1,1);
        }
    }
}