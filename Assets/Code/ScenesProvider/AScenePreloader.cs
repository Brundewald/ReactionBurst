using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyProject.ReactionBurst.Shared.ScenesProvider
{
    public abstract class AScenePreloader : MonoBehaviour
    {
        public abstract UniTask Preload();
        
        public abstract UniTask WaitForLoad();
        
        public abstract void OnLoaded();
    }
}