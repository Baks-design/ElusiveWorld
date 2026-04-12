using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class YieldCache
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
        static readonly Dictionary<float, WaitForSeconds> cache = new();
        static readonly Dictionary<float, WaitForSecondsRealtime> realtimeCache = new();
        const float PRECISION = 0.001f; // 1ms
        const int MAX_CACHE_SIZE = 256;

        [MethodImpl(INLINE)]
        public static WaitForSeconds Wait(this float seconds)
        {
            var key = Mathf.Round(seconds / PRECISION) * PRECISION;
            if (cache.TryGetValue(key, out var wait)) return wait;
            if (cache.Count >= MAX_CACHE_SIZE)cache.Clear();
            wait = new WaitForSeconds(key);
            cache[key] = wait;
            return wait;
        }

        public static WaitForSecondsRealtime WaitRealtime(this float seconds)
        {
            var key = Mathf.Round(seconds / PRECISION) * PRECISION;
            if (!realtimeCache.TryGetValue(key, out var wait))
            {
                wait = new WaitForSecondsRealtime(key);
                realtimeCache[key] = wait;
            }
            return wait;
        }
    }
}