using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Pixelation
{
    [Serializable, ExecuteInEditMode]
    public class PixelationController
    {
        [SerializeField] bool isEnabled = true;
        [SerializeField] float widthPixelation = 512f;
        [SerializeField] float heightPixelation = 256f;
        [SerializeField] float colorPrecision = 16f;
        Pixelation pixelation;
        VolumeProfile profile;

        public void Initialize(VolumeProfile profile) => this.profile = profile;

        public void Update()
        {
            if (!isEnabled) return;
            if (profile == null) return;
            if (pixelation == null) profile.TryGet(out pixelation);
            if (pixelation == null) return;

            pixelation.widthPixelation.value = widthPixelation;
            pixelation.heightPixelation.value = heightPixelation;
            pixelation.colorPrecision.value = colorPrecision;
        }
    }
}