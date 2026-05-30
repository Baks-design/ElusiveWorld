using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class Vector3Extensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Frame-rate independent exponential decay toward target.
        /// Best for smoothing (camera, velocity, position).
        /// </summary>
        [MethodImpl(INLINE)]
        public static Vector3 ExpDecay(this Vector3 a, Vector3 b, float rate, float dt) =>
            b + (a - b) * Mathf.Exp(-rate * dt);

        /// <summary>
        /// Multiplicative (geometric) interpolation between two vectors.
        /// Only valid for strictly positive values (e.g. scale, zoom).
        /// Falls back to Lerp if invalid.
        /// </summary>
        [MethodImpl(INLINE)]
        public static Vector3 Eerp(this Vector3 a, Vector3 b, float t)
        {
            // Fast paths
            if (t <= 0f) return a;
            if (t >= 1f) return b;

            // Domain check (required for log)
            if (a.x <= 0f || a.y <= 0f || a.z <= 0f ||
                b.x <= 0f || b.y <= 0f || b.z <= 0f)
                return Vector3.Lerp(a, b, t);

            return new Vector3(
                a.x * Mathf.Pow(b.x / a.x, t),
                a.y * Mathf.Pow(b.y / a.y, t),
                a.z * Mathf.Pow(b.z / a.z, t)
            );
        }

        /// <summary>
        /// Per-axis inverse of geometric interpolation.
        /// Returns independent t values per component.
        /// NOT a true inverse of Eerp unless all components share same ratio.
        /// </summary>
        [MethodImpl(INLINE)]
        public static Vector3 InverseEerpPerAxis(this Vector3 a, Vector3 b, Vector3 v) =>
            new(
                ComputeInverse(a.x, b.x, v.x),
                ComputeInverse(a.y, b.y, v.y),
                ComputeInverse(a.z, b.z, v.z)
            );

        [MethodImpl(INLINE)]
        static float ComputeInverse(float a, float b, float v)
        {
            if (a <= 0f || b <= 0f || v <= 0f || a == b)return 0f;
            return Mathf.Log(v / a) / Mathf.Log(b / a);
        }
    }
}