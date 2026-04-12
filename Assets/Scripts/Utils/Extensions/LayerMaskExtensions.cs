using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class LayerMaskExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Checks if the GameObject's layer is included in the mask.
        /// </summary>
        [MethodImpl(INLINE)]
        public static bool Contains(this LayerMask mask, GameObject gameObject)
        {
            if (gameObject == null) return false;
            var layer = gameObject.layer;
            return (mask.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// Checks if the given layer index is included in the mask.
        /// </summary>
        [MethodImpl(INLINE)]
        public static bool Contains(this LayerMask mask, int layer)
        {
            if ((uint)layer > 31) return false;
            return (mask.value & (1 << layer)) != 0;
        }

        [MethodImpl(INLINE)]
        public static LayerMask Add(this LayerMask mask, int layer)
        {
            mask.value |= 1 << layer;
            return mask;
        }

        [MethodImpl(INLINE)]
        public static LayerMask Remove(this LayerMask mask, int layer)
        {
            mask.value &= ~(1 << layer);
            return mask;
        }

        [MethodImpl(INLINE)]
        public static LayerMask Toggle(this LayerMask mask, int layer)
        {
            mask.value ^= 1 << layer;
            return mask;
        }
    }
}