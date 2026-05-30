using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class GameObjectExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Gets the component if it exists, otherwise adds it.
        /// </summary>
        [MethodImpl(INLINE)]
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (!gameObject.TryGetComponent<T>(out var component))
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}