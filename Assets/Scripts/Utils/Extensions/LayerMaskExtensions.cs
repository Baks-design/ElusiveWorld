using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class LayerMaskExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static bool ContainsLayer(this LayerMask layerMask, GameObject gameObject)
            => (layerMask.value & (1 << gameObject.layer)) != 0;
    }
}