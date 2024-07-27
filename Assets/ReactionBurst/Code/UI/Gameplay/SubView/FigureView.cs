using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.UI.Gameplay.Visualisation;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Gameplay.SubView
{
    [AddComponentMenu(NameConstants.NamespaceGame + "." + nameof(FigureView))]
    public class FigureView: MonoBehaviour
    {
        [SerializeField] private Image _figureImage;
        [SerializeField] private FigureVisualizer _visualizer;

        public Image FigureImage => _figureImage;
        public void SetImage(Sprite sprite, Color color)
        {
            _figureImage.sprite = sprite;
            _figureImage.color = color;
        }

        public async UniTask ShowAnswerResult(bool result, string modifier = "")
        {
            await _visualizer.ShowAnswerResult(result, modifier);
        }

        public void ResetView() => _visualizer.ResetView();
    }
}