using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Data;

namespace MyProject.ReactionBurst.Core.SubSystems
{
    public sealed class CompareService
    {
        private ComparableFigure _previousFigure;
        private ComparableFigure _currentComparable;

        public CompareService()
        {
            _previousFigure = null;
        }

        public void SetComparable(ComparableFigure comparableFigure)
        {
            _previousFigure ??= comparableFigure;
        }
        
        public async UniTask<bool> Compare(ComparableFigure currentFigure)
        {
            var sameFigures = _previousFigure.Sprite.Equals(currentFigure.Sprite) 
                              && _previousFigure.Color.Equals(currentFigure.Color);
            _previousFigure = currentFigure;
            return sameFigures;
        }

        public void Clear()
        {
            _previousFigure = null;
            _currentComparable = null;
        }
    }
}