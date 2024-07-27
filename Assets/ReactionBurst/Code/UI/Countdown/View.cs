using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Constants;
using UnityEngine;
using UnityEngine.Assertions;
using MyProject.ReactionBurst.UI.Visualization.Countdown;
using MyProject.ReactionBurst.UI;
using TMPro;

namespace MyProject.ReactionBurst.UI.Countdown
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + NameConstants.GameStartView)]
    public sealed class View : BaseView
    {
        [SerializeField] private TextMeshProUGUI _textField;
        
        [Header("Countdown Visualization")]
        [SerializeField] private CountdownVisualization _countdownVisualization;
        
        public override void Prepare<T>(T data)
        {
            var model = data as Model;
            Assert.IsNotNull(model);

            _textField.text = model.CountdownNumber.ToString();
            _countdownVisualization.Prepare();
        }
        
        public override void UpdateData(string dataName, object value)
        {
            if (dataName.Equals("CountdownNumber"))
            {
                OnCountdownChanged((int) value);
            }

            if (dataName.Equals("NeedClose"))
            {
                CloseVisualization().Forget();
            }
        }

        public async UniTask PlayOpenVisualisationAsync() => 
            await _countdownVisualization.ShowNotationVisualizationAsync();

        public void OnCountdownChanged(int countdownNumber)
        {
            _textField.text = countdownNumber.ToString();
            _countdownVisualization.Prepare();
            _countdownVisualization.ShowCountDownVisualization();
        }

        private async UniTask CloseVisualization()
        {
            await _countdownVisualization.CloseVisualizationAsync();
            OnCloseButtonPressed();
        }
    }
}