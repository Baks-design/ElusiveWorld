using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Pixelation
{
    public class Pixelation : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter widthPixelation = new(512f);
        public FloatParameter heightPixelation = new(512f);
        public FloatParameter colorPrecision = new(32f);
        
        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}