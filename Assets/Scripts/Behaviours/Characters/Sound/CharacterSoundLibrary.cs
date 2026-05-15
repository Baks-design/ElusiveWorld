using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Data/Systems/Audio/Sound Library")]
    public class CharacterSoundLibrary : ScriptableObject
    {
        [SerializeField] SoundData concreteGroundFootstep;

        public SoundData[] GetClips => new SoundData[]
        {
            concreteGroundFootstep
        };
    }
}