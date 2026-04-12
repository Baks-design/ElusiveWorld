using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class FloatExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static float ExpDecay(this float a, float b, float rate, float dt) =>
            b + (a - b) * Mathf.Exp(-rate * dt);

        [MethodImpl(INLINE)]
        public static float ExpDecayInverse(float a, float b, float value, float rate)
        {
            if (a == b) return 0f;
            return -Mathf.Log((value - b) / (a - b)) / rate;
        }

        /// <summary>
        /// Exponential interpolation, the multiplicative version of lerp, 
        /// useful for values such as scaling or zooming
        /// </summary>
        /// <param name="a">The start value</param>
        /// <param name="b">The end value</param>
        /// <param name="t">The t-value from 0 to 1 representing position along the eerp</param>
        [MethodImpl(INLINE)]
        public static float Eerp(this float a, float b, float t)
        {
            if (a <= 0f || b <= 0f) return Mathf.Lerp(a, b, t);
            return a * Mathf.Pow(b / a, t);
        }

        /// <summary>
        /// Inverse exponential interpolation, the multiplicative version of InverseLerp, 
        /// useful for values such as scaling or zooming
        /// </summary>
        /// <param name="a">The start value</param>
        /// <param name="b">The end value</param>
        /// <param name="v">
        /// A value between a and b. Note: values outside this range are still valid, 
        /// and will be extrapolated</param>
        [MethodImpl(INLINE)]
        public static float InverseEerp(this float a, float b, float v)
        {
            if (a <= 0f || b <= 0f || v <= 0f || a == b) return 0f;
            return Mathf.Log(v / a) / Mathf.Log(b / a);
        }
    }
}