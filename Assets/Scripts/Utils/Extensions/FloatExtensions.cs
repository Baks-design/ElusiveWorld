using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class FloatExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static float ExpDecay(this float a, float b, float decay, float deltaTime) =>
            b + (a - b) * Mathf.Exp(-decay * deltaTime);

        /// <summary>
        /// Exponential interpolation, the multiplicative version of lerp, 
        /// useful for values such as scaling or zooming
        /// </summary>
        /// <param name="a">The start value</param>
        /// <param name="b">The end value</param>
        /// <param name="t">The t-value from 0 to 1 representing position along the eerp</param>
        [MethodImpl(INLINE)]
        public static float Eerp(this float a, float b, float t) =>
            t switch
            {
                0f => a,
                1f => b,
                _ => a * Mathf.Exp(Mathf.Log(b / a) * t) 
            };

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
            if (v == a) return 0f;
            if (v == b) return 1f;
            return Mathf.Log(v / a) / Mathf.Log(b / a);
        }
    }
}