
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI;
using UnityEngine.Localization;

namespace MyProject.ReactionBurst.UI.Gameplay
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

        private LocalizedString _notation;
        
        public LocalizedString Notation
        {
            get => _notation;
            set
            {
                _notation = value;
                OnPropertyChanged(_notation);
            }
        }

        private ComparableFigure[] _initialFiguresPool;

        public ComparableFigure[] InitialFiguresPool
        {
            get => _initialFiguresPool;
            set
            {
                _initialFiguresPool = value;
                OnPropertyChanged(_initialFiguresPool);
            }
        }

        private ComparableFigure _figure;

        public ComparableFigure Figure
        {
            get => _figure;
            set
            {
                _figure = value;
                OnPropertyChanged(_figure);
            }
        }

        private int _scoreModifier;

        public int ScoreModifier
        {
            get => _scoreModifier;
            set
            {
                if (_scoreModifier != value)
                {
                    _scoreModifier = value;
                    OnPropertyChanged(_scoreModifier);
                }
            }
        }

        //public Model()
        //{
        //    LocalizationManager.OnLocalizeEvent += OnLocalizationChanged;
        //}
        //
        //private void OnLocalizationChanged()
        //{
        //    OnPropertyChanged(_notation, nameof(Notation));
        //}
    }
}