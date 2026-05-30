using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Data/Systems/Audio/SoundData")]
    public class SoundData : ScriptableObject
    {
        public AudioClip[] clips;
        public SoundDataSettings settings;
        public bool frequentSound;
        public float volume = 1f;
        public float pitch = 1f;

        public AudioClip GetClip()
        {
            if (clips == null || clips.Length == 0)  return null;
            return clips.Length == 1 ? clips[0] : clips[Random.Range(0, clips.Length)];
        }
    }
}