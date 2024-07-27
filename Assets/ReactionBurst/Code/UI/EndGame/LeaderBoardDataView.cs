using MyProject.ReactionBurst.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.EndGame
{
    public class LeaderBoardDataView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _playerHighestScore;
        [SerializeField] private TextMeshProUGUI _playerLeaderboardScore;

        public void SetLeaderBoardData(LeaderBoardData leaderBoardData)
        {
            _playerName.text = leaderBoardData.Name;
            _playerHighestScore.text = leaderBoardData.HighestScore.ToString();
            _playerLeaderboardScore.text = leaderBoardData.LeaderboardNumber.ToString();
        }
    }
}