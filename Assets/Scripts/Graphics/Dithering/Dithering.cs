using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Dithering
{
    public class Dithering : VolumeComponent, IPostProcessComponent
    {
        public IntParameter patternIndex = new(0);
        public FloatParameter ditherThreshold = new(512f);
        public FloatParameter ditherStrength = new(1f);
        public FloatParameter ditherScale = new(2f);
        
        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}