using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Dithering
{
    [Serializable, ExecuteInEditMode]
    public class DitheringController
    {
        [SerializeField] bool isEnabled = true;
        [SerializeField] int patternIndex = 0;
        [SerializeField] float ditherThreshold = 1f;
        [SerializeField] float ditherStrength = 1f;
        [SerializeField] float ditherScale = 2f;
        Dithering dithering;
        VolumeProfile profile;

        public void Initialize(VolumeProfile profile) => this.profile = profile;

        public void Update()
        {
            if (!isEnabled) return;
            if (profile == null) return;
            if (dithering == null) profile.TryGet(out dithering);
            if (dithering == null) return;

            dithering.patternIndex.value = patternIndex;
            dithering.ditherThreshold.value = ditherThreshold;
            dithering.ditherStrength.value = ditherStrength;
            dithering.ditherScale.value = ditherScale;
        }
    }
}