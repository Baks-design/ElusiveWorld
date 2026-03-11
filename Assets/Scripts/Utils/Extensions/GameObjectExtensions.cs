using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class GameObjectExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();
            return component;
        }
    }
}