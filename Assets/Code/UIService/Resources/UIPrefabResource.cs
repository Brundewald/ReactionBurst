using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyProject.UIService.Resource
{
    [Serializable]
    public class UIPrefabResource
    {
        public string Name;
        public IEnumerable<MonoBehaviour> Components;
        public AssetReferenceGameObject PrefabReference;

#if UNITY_EDITOR
        public void Validate()
        {
            if(PrefabReference.editorAsset is null)
                return;
            
            Name = PrefabReference.editorAsset.name;
            Components = PrefabReference.editorAsset.GetComponents<MonoBehaviour>();
        }
#endif
    }
}