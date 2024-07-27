namespace MyProject.ReactionBurst.UI.Pause
{
    public class Model : BaseModel
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
        
        public static string NeedCloseKey => nameof(NeedClose);

        private bool _sfxEnabled;

        public bool SFXEnabled
        {
            get => _sfxEnabled;
            set
            {
                _sfxEnabled = value;
                OnPropertyChanged(_sfxEnabled);
            }
        }
        
        public static string SFXEnabledKey => nameof(SFXEnabled);
        
        private bool _musicEnabled;

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                OnPropertyChanged(_musicEnabled);
            }
        }
        
        public static string MusicEnabledKey => nameof(MusicEnabled);
    }
}