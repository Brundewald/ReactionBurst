using System;
using System.Threading;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using MyProject.ReactionBurst.Constants;
using UnityEngine;

namespace MyProject.ReactionBurst.UI.Visualization.Countdown
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + nameof(CountdownVisualization))]
    public sealed class CountdownVisualization : MonoBehaviour
    {
        [SerializeField] private Transform _countDownTransform;
        [SerializeField] private CanvasGroup _notationGroup;
        
        [Header("View Fade Visualization")]
        [SerializeField] private float _fadeInDuration = 0.25f;
        //[SerializeField] private Ease _fadeInEase = Ease.OutSine;
        [SerializeField] private float _fadeOutDuration = 0.15f;
        //[SerializeField] private Ease _fadeOutEase = Ease.InSine;
        [SerializeField] private float _fadeStayDuration = 0.5f;
        
        [Header("View Scale Visualization")]
        [SerializeField] private AnimationCurve _scaleInCurve;
        [SerializeField] private float _scaleInDuration = 0.15f;

        private bool _alreadyShown;
        private CancellationToken _token;

        public void Prepare()
        {
            _countDownTransform.transform.localScale = Vector3.zero;
        }

        private void Awake()
        {
            _token = gameObject.GetCancellationTokenOnDestroy();
        }

        private void OnEnable()
        {
            _notationGroup.alpha = 0;
            _notationGroup.transform.localScale = Vector3.zero;
        }

        public async UniTask ShowNotationVisualizationAsync()
        {
            LMotion
                .Create(Vector3.zero, Vector3.one, _fadeInDuration)
                .WithEase(_scaleInCurve)
                .BindToLocalScale(_notationGroup.transform)
                .AddTo(this)
                .RegisterActiveHandle();

            await LMotion
                .Create(0f, 1f, _fadeInDuration)
                .WithEase(_scaleInCurve)
                .BindToCanvasGroupAlpha(_notationGroup)
                .AddTo(this)
                .RegisterActiveHandle();
        }

        public async void ShowCountDownVisualization()
        {
            await LMotion.Create(Vector3.zero, Vector3.one, _scaleInDuration)
                .WithEase(_scaleInCurve)
                .BindToLocalScale(_countDownTransform)
                .AddTo(this)
                .RegisterActiveHandle();;

            await UniTask.Delay(TimeSpan.FromSeconds(_fadeStayDuration), cancellationToken: _token)
                .SuppressCancellationThrow();
            
            await LMotion.Create(Vector3.one, Vector3.zero, _scaleInDuration)
                .WithEase(_scaleInCurve)
                .BindToLocalScale(_countDownTransform)
                .AddTo(this)
                .RegisterActiveHandle();;
        }

        public async UniTask CloseVisualizationAsync()
        {
            await LMotion
                .Create(1f, 0f, _fadeInDuration)
                .WithEase(_scaleInCurve)
                .BindToCanvasGroupAlpha(_notationGroup)
                .AddTo(this)
                .RegisterActiveHandle();;
        }
    }
}