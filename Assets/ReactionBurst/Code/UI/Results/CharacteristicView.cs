using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Results
{
    public class CharacteristicView : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _name;
        
        [SerializeField] 
        private TextMeshProUGUI _oldScore;
        [SerializeField] 
        private Slider _oldScoreProgress;
        
        //[SerializeField] 
        //private TextMeshBENumber _addScore;
        [SerializeField] 
        private Slider _addScoreProgress;

        private float _currentProgress;
        private float _oldProgress;

        public void Init(CharacteristicVisualData visualData, CharacteristicData newData, int newResult, CharacteristicData oldData, int oldScore)
        {
            _name.text = visualData.Name;
            
            _currentProgress = newData.Progress;
            _oldProgress = oldData.Progress;

            //_addScore.Init(BENumber.IncType.VALUE, "+ #########", 0, _currentProgress, 0, 50);
            //_addScore.Color = visualData.SecondaryColor;
            _addScoreProgress.maxValue = newData.MaxValue;
            _addScoreProgress.value = 0f;
            _addScoreProgress.fillRect.GetComponent<Image>().color = visualData.SecondaryColor;
            
            _oldScore.text = oldScore.ToString();
            _oldScore.color = visualData.PrimaryColor;
            _oldScoreProgress.maxValue = oldData.MaxValue;
            _oldScoreProgress.value = 0;
            _oldScoreProgress.fillRect.GetComponent<Image>().color = visualData.PrimaryColor;
        }

        private void OnEnable()
        {
            Visualize(_oldProgress, _currentProgress);
        }

        private void Visualize(float oldProgress, float newProgress)
        { 
            //_oldScoreProgress.DOValue(oldProgress, 0.5f);
            //_addScoreProgress.DOValue(newProgress, 1);
            //_addScore.ChangeTo(_currentProgress);
        }
        
    }

    public class CharacteristicVisualData
    {
        public string Name { get; set; }
        public Color SecondaryColor { get; set; }
        public Color PrimaryColor { get; set; }
    }
}
