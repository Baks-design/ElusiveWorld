using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class AudioExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
        const float MIN_VOLUME = 0.0001f; // ≈ -80 dB

        [MethodImpl(INLINE)]
        public static float ToDecibel(this float sliderValue)
        {
            var value = Mathf.Clamp(sliderValue, MIN_VOLUME, 1f);
            return Mathf.Log10(value) * 20f;
        }

        [MethodImpl(INLINE)]
        public static float ToPerceptualCurve(this float fraction)
        {
            fraction = Mathf.Clamp01(fraction);
            return Mathf.Log10(1f + 9f * fraction);
        }

        [MethodImpl(INLINE)]
        public static float ToLinearVolume(this float db) => Mathf.Pow(10f, db / 20f);

        [MethodImpl(INLINE)]
        public static float FromPerceptualCurve(this float value)
        {
            value = Mathf.Clamp01(value);
            return (Mathf.Pow(10f, value) - 1f) / 9f;
        }
    }
}