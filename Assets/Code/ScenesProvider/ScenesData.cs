using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyProject.ReactionBurst.Shared.ScenesProvider
{
    [CreateAssetMenu]
    public class ScenesData : ScriptableObject
    {
        [SerializeField]
        private List<SceneData> Scenes;

        public AssetReference GetByName(string sceneName)
        {
            return Scenes.Find(s => s.Name.Equals(sceneName)).Asset;
        }
    }
}