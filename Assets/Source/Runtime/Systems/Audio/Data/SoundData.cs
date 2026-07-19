using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Data/Systems/Audio/Sound Data")]
    public class SoundData : ScriptableObject
    {
        public bool frequentSound;
        public float volume = 1f;
        public float pitch = 1f;
        public SoundDataSettings settings;
        public AudioClip[] clips;

        public AudioClip GetClip()
        {
            if (clips == null || clips.Length == 0)  return null;
            return clips.Length == 1 ? clips[0] : clips[Random.Range(0, clips.Length)];
        }
    }
}