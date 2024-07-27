using MyProject.ReactionBurst.Data;

namespace MyProject.ReactionBurst.UI.EndGame
{
    public class Model : BaseModel
    {
        private LeaderBoardData _playerLeaderboardData;

        public LeaderBoardData PlayerLeaderboardData
        {
            get => _playerLeaderboardData;
            set
            {
                _playerLeaderboardData = value;
                OnPropertyChanged(_playerLeaderboardData);
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