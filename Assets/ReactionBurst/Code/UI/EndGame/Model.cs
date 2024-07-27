using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.Games.RecallRun.EndGame
{
    public class Model : BaseModel
    {
        private int _finalScore;

        public int FinalScore
        {
            get => _finalScore;
            set
            {  
                _finalScore = value;
                OnPropertyChanged(_finalScore);
            }
        }
        
        private LeaderBoardData _playerLeaderboardScore;

        public LeaderBoardData PlayerLeaderboardScore
        {
            get => _playerLeaderboardScore;
            set
            {
                _playerLeaderboardScore = value;
                OnPropertyChanged(_playerLeaderboardScore);
            }
        }

        private LeaderBoardData[] _leaderboardData;

        public LeaderBoardData[] LeaderboardData
        {
            get => _leaderboardData;
            set
            {
                _leaderboardData = value;
                OnPropertyChanged(_leaderboardData);
            }
        }
    }
}