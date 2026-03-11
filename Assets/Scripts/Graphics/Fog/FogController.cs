using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Fog
{
    [Serializable, ExecuteInEditMode]
    public class FogController
    {
        [SerializeField] bool isEnabled = true;
        [SerializeField, Range(0f, 50f)] float fogDensity = 1f;
        [SerializeField, Range(0f, 1000f)] float fogDistance = 10f;
        [SerializeField, Range(0f, 100f)] float fogNear = 1f;
        [SerializeField, Range(0f, 100f)] float fogFar = 100f;
        [SerializeField, Range(0f, 100f)] float fogAltScale = 10f;
        [SerializeField, Range(0f, 1000f)] float fogThinning = 100f;
        [SerializeField, Range(0f, 1000f)] float noiseScale = 100f;
        [SerializeField, Range(0f, 1)] float noiseStrength = 0.05f;
        [SerializeField] Color fogColor;
        [SerializeField] Color ambientColor;
        Fog fog;
        VolumeProfile profile;

        public float GetFogDistance => fogDistance;

        public void Initialize(VolumeProfile profile) => this.profile = profile;

        public void Update()
        {
            if (!isEnabled) return;
            if (profile == null) return;
            if (fog == null) profile.TryGet(out fog);
            if (fog == null) return;

            fog.fogDensity.value = fogDensity;
            fog.fogDistance.value = fogDistance;
            fog.fogNear.value = fogNear;
            fog.fogFar.value = fogFar;
            fog.fogAltScale.value = fogAltScale;
            fog.fogThinning.value = fogThinning;
            fog.noiseScale.value = noiseScale;
            fog.noiseStrength.value = noiseStrength;
            fog.fogColor.value = fogColor;
            fog.ambientColor.value = ambientColor;
        }

        public float SetFogDistance(float value) => fogDistance = value;
    }
}