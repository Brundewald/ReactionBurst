namespace MyProject.ReactionBurst.UI.TopBar
{
    public sealed class Model : BaseModel
    {
        private bool _needClose;
        
        public bool NeedClose
        {
            get => _needClose;
            set
            {
                _needClose = value;
                OnPropertyChanged(_needClose);
            }
        }

        private int _timerLeft;

        public int TimerLeft
        {
            get => _timerLeft;
            set
            {
                if (_timerLeft != value)
                {
                    _timerLeft = value;
                    OnPropertyChanged(_timerLeft);
                }
            }
        }

        private int _score;

        public int Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged(_score);
                }
            }
        }

        private bool _isGameMode;

        public bool IsGameMode
        {
            get => _isGameMode;
            set
            {
                if (_isGameMode != value)
                {
                    _isGameMode = value;
                    OnPropertyChanged(_isGameMode);
                }
            }
        }

        private int _addScoreValue;

        public int AddScoreValue
        {
            get => _addScoreValue;
            set
            {
                _addScoreValue = value;
                OnPropertyChanged(_addScoreValue);
            }
        }
    }
}