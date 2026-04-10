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

        /// <summary>
        /// Checks if the given layer number is contained in the LayerMask.
        /// </summary>
        /// <param name="mask">The LayerMask to check.</param>
        /// <param name="layerNumber">The layer number to check if it is contained in the LayerMask.</param>
        /// <returns>True if the layer number is contained in the LayerMask, otherwise false.</returns>
        [MethodImpl(INLINE)]
        public static bool Contains(this LayerMask mask, int layerNumber)
            => mask == (mask | (1 << layerNumber));
    }
}