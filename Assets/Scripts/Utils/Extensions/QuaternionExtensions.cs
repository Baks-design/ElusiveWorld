using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class QuaternionExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Frame-rate independent exponential smoothing for rotations.
        /// </summary>
        [MethodImpl(INLINE)]
        public static Quaternion ExpDecay(this Quaternion a, Quaternion b, float rate, float dt)
        {
            // Compute exponential interpolation factor
            var t = 1f - Mathf.Exp(-rate * dt);
            // Use dot to detect small angle (faster than Angle)
            var dot = Quaternion.Dot(a, b);
            // If very close, use Lerp (faster, avoids precision issues)
            if (dot > 0.9995f) return Quaternion.Lerp(a, b, t);
            return Quaternion.Slerp(a, b, t);
        }
    }
}