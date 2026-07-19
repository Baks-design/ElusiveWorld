using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Data/Systems/Audio/Sound Library")]
    public class CharacterSoundLibrary : ScriptableObject
    {
        [field: SerializeField] public SoundData ConcreteSounds { get; set; }
        [field: SerializeField] public SoundData MuddySounds { get; set; }
    }
}