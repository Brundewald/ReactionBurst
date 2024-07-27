using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.Countdown
{
    public sealed class Model : BaseModel
    {
        private int _countDown = 3;
        
        public int CountdownNumber
        {
            get => _countDown;
            set
            {
                _countDown = value;
                OnPropertyChanged(_countDown);
            }
        }


        public bool NeedClose
        {
            get => _needClose;
            set
            {
                _needClose = value;
                OnPropertyChanged(_needClose);
            }
        }
        
        private bool _needClose;
    }
}