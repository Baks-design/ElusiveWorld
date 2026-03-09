using System;
using Assets.Scripts.Internal.Runtime.Core.Systems.Audio;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Sound.Data
{
    [Serializable]
    public class GroundTypeSettings
    {
        public string groundTypeName;
        public SoundLibraryObject soundData;
        public float volume = 1f;
        public float pitchVariation = 0.1f;
        public LayerMask groundLayers;
    }
}