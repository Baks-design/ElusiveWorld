using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class IEnumeratorExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        static readonly Dictionary<float, WaitForSeconds> waitForSecondsCache = new();

        [MethodImpl(INLINE)]
        public static WaitForSeconds Wait(float seconds)
        {
            if (!waitForSecondsCache.TryGetValue(seconds, out var wait))
            {
                wait = new WaitForSeconds(seconds);
                waitForSecondsCache[seconds] = wait;
            }
            return wait;
        }
    }
}