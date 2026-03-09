using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Sound.Data
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