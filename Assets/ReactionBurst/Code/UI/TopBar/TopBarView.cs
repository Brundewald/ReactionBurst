using System;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.UI.TopBar.Visualisation;
using MyProject.ReactionBurst.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


namespace MyProject.ReactionBurst.UI.TopBar
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + NameConstants.TopBarView)]
    public sealed class TopBarView : BaseView
    {
        [SerializeField] private TextMeshProUGUI _timerField;
        [SerializeField] private TextMeshProUGUI _scoreCounter;
        [SerializeField] private TextMeshProUGUI _additionalScoreField;
        [SerializeField] private TopBarVisualiser _topBarVisualiser;

        private int _previousModifier;

        public override void Prepare<T>(T data)
        {
            var model = data as Model;
            Assert.IsNotNull(model);
            ResetView();
            _topBarVisualiser.ResetView();
        }

        private void OnEnable() => 
            _topBarVisualiser.PlayIntroAnimation().Forget();

        public override void UpdateData(string dataName, object value)
        {
            if (dataName.Equals("Score"))
            {
                _scoreCounter.text = value.ToString();
                _topBarVisualiser.PlayAddScoreAnimationAsync().Forget();
            }

            if (dataName.Equals("TimerLeft"))
            {
                UpdateTimer(value);
            }

            if (dataName.Equals("AddScoreValue"))
            {
                _additionalScoreField.text = $"+{value}";
            }
        }

        public void UpdateTimer(object value)
        {
            var timeValue = (int)value;
            var time = TimeSpan.FromSeconds(timeValue);
            _timerField.text = $"{time.Minutes} : {time.Seconds:D2}";
        }

        private void OnDisable() 
            => ResetView();

        private void ResetView()
        {
            _topBarVisualiser.ResetView();
            _scoreCounter.text = "0";
            _timerField.text = "0:00";
        }
    }
}
