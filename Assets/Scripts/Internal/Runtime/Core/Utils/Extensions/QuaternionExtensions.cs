using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Extensions
{
    public static class QuaternionExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static Quaternion ExpDecay(Quaternion a, Quaternion b, float decay, float deltaTime)
        {
            if (Quaternion.Angle(a, b) < 0.01f) return b;
            var t = 1f - Mathf.Exp(-decay * deltaTime);
            return Quaternion.Slerp(a, b, t);
        }
    }
}