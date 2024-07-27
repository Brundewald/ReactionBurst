using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyProject.Resource
{
    [CreateAssetMenu(fileName = nameof(PrefabProvider), menuName = "Configs/" + nameof(PrefabProvider))]
    public class PrefabProvider : ScriptableObject
    {
        [SerializeField] private List<PrefabResource> prefabs;

        public AssetReference GetPrefabReference<T>()
        {
            return GetPrefabReference(typeof(T));
        }

        public AssetReference GetPrefabReference(Type type)
        {
            foreach (var prefab in prefabs)
            {
                if (prefab.Components.Any(type.IsInstanceOfType))
                    return prefab.PrefabReference;
            }
            
            return null;
        }

        public AssetReference GetPrefabReference(string name)
        {
            foreach (var prefab in prefabs)
            {
                if (prefab.Name == name)
                    return prefab.PrefabReference;
            }
            
            return null;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            foreach (var prefab in prefabs)
            {
                prefab.Validate();
            }
        }
#endif
    }
}