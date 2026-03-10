using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class QuaternionExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static Quaternion ExpDecay(this Quaternion a, Quaternion b, float decay, float deltaTime) 
        {
            if (Quaternion.Angle(a, b) < 0.01f) return b;
            var t = 1f - Mathf.Exp(-decay * deltaTime);
            return Quaternion.Slerp(a, b, t);
        }
    }
}