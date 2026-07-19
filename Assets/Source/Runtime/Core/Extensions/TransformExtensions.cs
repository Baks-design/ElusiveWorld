using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class TransformExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static void Reset(this Transform transform) =>
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}