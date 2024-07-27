namespace MyProject.ReactionBurst.UI.NavigationPanel
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
    }
}