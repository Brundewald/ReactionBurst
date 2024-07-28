using UnityEngine;

namespace MyProject.ReactionBurst.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasProvider : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        public Canvas Canvas => _canvas;

        private void Awake()
        { 
            var worldCamera = Camera.main;
            _canvas.worldCamera = worldCamera;
        }
    }
}