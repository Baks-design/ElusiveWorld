using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class Vector2Extensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static Vector2 ExpDecay(this Vector2 a, Vector2 b, float rate, float dt) =>
            b + (a - b) * Mathf.Exp(-rate * dt);

        [MethodImpl(INLINE)]
        public static Vector2 Eerp(this Vector2 a, Vector2 b, float t)
        {
            if (a.x <= 0f || a.y <= 0f || b.x <= 0f || b.y <= 0f) 
                return Vector2.Lerp(a, b, t);
            return new Vector2(
                a.x * Mathf.Pow(b.x / a.x, t),
                a.y * Mathf.Pow(b.y / a.y, t)
            );
        }

        [MethodImpl(INLINE)]
        public static Vector2 InverseEerpPerAxis(this Vector2 a, Vector2 b, Vector2 v) =>
            new(
                (a.x > 0f && b.x > 0f && v.x > 0f && a.x != b.x)
                    ? Mathf.Log(v.x / a.x) / Mathf.Log(b.x / a.x)
                    : 0f,
                (a.y > 0f && b.y > 0f && v.y > 0f && a.y != b.y)
                    ? Mathf.Log(v.y / a.y) / Mathf.Log(b.y / a.y)
                    : 0f
            );
    }
}