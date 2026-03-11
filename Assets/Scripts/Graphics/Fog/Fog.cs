using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Fog
{
    public class Fog : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 10f)] public FloatParameter fogDensity = new(1f);
        [Range(0f, 100f)] public FloatParameter fogDistance = new(10f);
        public ColorParameter fogColor = new(Color.white);
        public ColorParameter ambientColor = new(new Color(0.1f, 0.1f, 0.1f, 0.1f));
        [Range(0f, 100f)] public FloatParameter fogNear = new(1f);
        [Range(0f, 100f)] public FloatParameter fogFar = new(100f);
        [Range(0f, 100f)] public FloatParameter fogAltScale = new(10f);
        [Range(0f, 1000f)] public FloatParameter fogThinning = new(100f);
        [Range(0f, 1000f)] public FloatParameter noiseScale = new(100f);
        [Range(0f, 1f)] public FloatParameter noiseStrength = new(0.05f);

        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}
