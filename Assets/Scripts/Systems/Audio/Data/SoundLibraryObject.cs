using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    [CreateAssetMenu(fileName = "SoundLibraryObject", menuName = "Data/Systems/Sound/SoundLibraryObject")]
    public class SoundLibraryObject : ScriptableObject
    {
        [SerializeField] SoundData concreteGroundFootstep;
        [SerializeField] SoundData muddyGroundFootstep;

        public SoundData[] GetClips => new SoundData[] { concreteGroundFootstep, muddyGroundFootstep };
    }
}