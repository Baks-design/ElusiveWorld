using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Extensions
{
    public static class Vector2Extensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float deltaTime) =>
            b + (a - b) * Mathf.Exp(-decay * deltaTime);

        /// <summary>Exponential interpolation, the multiplicative version of lerp, useful for values such as scaling or zooming</summary>
        /// <param name="a">The start value</param>
        /// <param name="b">The end value</param>
        /// <param name="t">The t-value from 0 to 1 representing position along the eerp</param>
        [MethodImpl(INLINE)]
        public static Vector2 Eerp(Vector2 a, Vector2 b, float t) =>
            t switch
            {
                0f => a,
                1f => b,
                _ => new Vector2(
                    FloatExtensions.Eerp(a.x, b.x, t),
                    FloatExtensions.Eerp(a.y, b.y, t)
                )
            };

        /// <summary>Inverse exponential interpolation, the multiplicative version of InverseLerp, useful for values such as scaling or zooming</summary>
        /// <param name="a">The start value</param>
        /// <param name="b">The end value</param>
        /// <param name="v">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
        [MethodImpl(INLINE)]
        public static Vector2 InverseEerp(Vector2 a, Vector2 b, Vector2 v)
        {
            if (v == a) return Vector2.zero;
            if (v == b) return Vector2.one;
            return new Vector2(
                Mathf.Log(v.x / a.x) / Mathf.Log(b.x / a.x),
                Mathf.Log(v.y / a.y) / Mathf.Log(b.y / a.y)
            );
        }
    }
}