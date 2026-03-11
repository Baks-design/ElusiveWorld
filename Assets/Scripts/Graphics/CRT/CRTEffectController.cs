using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.CRT
{
    [Serializable, ExecuteInEditMode]
    public class CRTEffectController
    {
        [SerializeField] bool isEnabled = true;
        [SerializeField] float scanlinesWeight = 1f;
        [SerializeField] float noiseWeight = 1f;
        [SerializeField] float screenBendX = 1000f;
        [SerializeField] float screenBendY = 1000f;
        [SerializeField] float vignetteAmount = 0f;
        [SerializeField] float vignetteSize = 0f;
        [SerializeField] float vignetteRounding = 0f;
        [SerializeField] float vignetteSmoothing = 0f;
        [SerializeField] float scanLinesDensity = 200f;
        [SerializeField] float scanLinesSpeed = -10f;
        [SerializeField] float noiseAmount = 250f;
        [SerializeField] Vector2 chromaticRed = new();
        [SerializeField] Vector2 chromaticGreen = new();
        [SerializeField] Vector2 chromaticBlue = new();
        [SerializeField] float grilleOpacity = 0.4f;
        [SerializeField] float grilleCounterOpacity = 0.2f;
        [SerializeField] float grilleResolution = 360f;
        [SerializeField] float grilleCounterResolution = 540f;
        [SerializeField] float grilleBrightness = 15f;
        [SerializeField] float grilleUvRotation = 90f;
        [SerializeField] float grilleUvMidPoint = 0.5f;
        [SerializeField] Vector3 grilleShift = new(1f, 1f, 1f);
        VolumeProfile profile;
        Crt crt;

        public void Initialize(VolumeProfile profile) => this.profile = profile;

        public void Update()
        {
            if (!isEnabled || profile == null) return;
            if (crt == null)
            {
                profile.TryGet(out crt);
                return;
            }

            crt.scanlinesWeight.value = scanlinesWeight;
            crt.noiseWeight.value = noiseWeight;
            crt.screenBendX.value = screenBendX;
            crt.screenBendY.value = screenBendY;
            crt.vignetteAmount.value = vignetteAmount;
            crt.vignetteSize.value = vignetteSize;
            crt.vignetteRounding.value = vignetteRounding;
            crt.vignetteSmoothing.value = vignetteSmoothing;
            crt.scanlinesDensity.value = scanLinesDensity;
            crt.scanlinesSpeed.value = scanLinesSpeed;
            crt.noiseAmount.value = noiseAmount;
            crt.chromaticRed.value = chromaticRed;
            crt.chromaticGreen.value = chromaticGreen;
            crt.chromaticBlue.value = chromaticBlue;
            crt.grilleOpacity.value = grilleOpacity;
            crt.grilleCounterOpacity.value = grilleCounterOpacity;
            crt.grilleResolution.value = grilleResolution;
            crt.grilleCounterResolution.value = grilleCounterResolution;
            crt.grilleBrightness.value = grilleBrightness;
            crt.grilleUvRotation.value = grilleUvRotation;
            crt.grilleUvMidPoint.value = grilleUvMidPoint;
            crt.grilleShift.value = grilleShift;
        }
    }
}