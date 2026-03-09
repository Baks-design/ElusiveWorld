using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Audio
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Data/Systems/Sound/SoundData")]
    public class SoundData : ScriptableObject
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop = false;
        public bool playOnAwake = true;
        public bool frequentSound;

        public bool mute = false;
        public bool bypassEffects = false;
        public bool bypassListenerEffects = false;
        public bool bypassReverbZones = false;

        public int priority = 128;
        public float volume = 1f;
        public float pitch = 1f;
        public float panStereo = 0f;
        public float spatialBlend = 0f;
        public float reverbZoneMix = 1f;
        public float dopplerLevel = 1f;
        public float spread = 0f;

        public float minDistance = 1f;
        public float maxDistance = 500f;

        public bool ignoreListenerVolume = true;
        public bool ignoreListenerPause = true;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    }
}