using MyProject.ReactionBurst.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.Games.RecallRun.EndGame
{
    public class View : BaseView
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _leaderBoardButton;
        [SerializeField] private Button _resultButton;
        
        [SerializeField] private TextMeshProUGUI _highestScore;
        [SerializeField] private TextMeshProUGUI _playerScore;
        [SerializeField] private TextMeshProUGUI _playerLeaderboardScore;
        
        [SerializeField] private LeaderBoardDataView _leaderBoardPrefab;
        [SerializeField] private ScrollView _scrollView;
        [SerializeField] private Visualization _visualization;
        
        public override void Prepare<T>(T data)
        {
            base.Prepare(data);
        }

        public override void UpdateData(string dataName, object value)
        {
            if (dataName == nameof(Model.FinalScore))
            {
                
            }

            if (dataName == nameof(Model.LeaderboardData))
            {
                
            }

            if (dataName == nameof(Model.LeaderboardData))
            {
                
            }
        }
    }
}