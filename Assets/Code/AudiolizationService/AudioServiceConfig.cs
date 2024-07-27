using System.Collections.Generic;
using UnityEngine;

namespace MyProject.ReactionBurst.AudiolizationService
{
    [CreateAssetMenu(fileName = nameof(AudioServiceConfig), menuName = "Configs/" + nameof(AudioServiceConfig))]
    public class AudioServiceConfig : ScriptableObject
    {
        [field: SerializeField] public List<string> SFXKeys;
        [field: SerializeField] public List<string> MusicKeys;
    }
}