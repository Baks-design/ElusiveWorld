using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.CRT
{
    public class Crt : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter scanlinesWeight = new(1f);
        public FloatParameter noiseWeight = new(1f);
        public FloatParameter screenBendX = new(1000f);
        public FloatParameter screenBendY = new(1000f);
        public FloatParameter vignetteAmount = new(0f);
        public FloatParameter vignetteSize = new(2f);
        public FloatParameter vignetteRounding = new(2f);
        public FloatParameter vignetteSmoothing = new(1f);
        public FloatParameter scanlinesDensity = new(200f);
        public FloatParameter scanlinesSpeed = new(-10f);
        public FloatParameter noiseAmount = new(250f);
        public Vector2Parameter chromaticRed = new(new Vector2());
        public Vector2Parameter chromaticGreen = new(new Vector2());
        public Vector2Parameter chromaticBlue = new(new Vector2());
        public FloatParameter grilleOpacity = new(0.4f);
        public FloatParameter grilleCounterOpacity = new(0.2f);
        public FloatParameter grilleResolution = new(360f);
        public FloatParameter grilleCounterResolution = new(540f);
        public FloatParameter grilleUvRotation = new(90f);
        public FloatParameter grilleBrightness = new(15f);
        public FloatParameter grilleUvMidPoint = new(0.5f);
        public Vector3Parameter grilleShift = new(new Vector3(1f, 1f, 1f));

        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}