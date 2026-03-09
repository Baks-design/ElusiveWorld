using System;
using UnityEngine;
using LitMotion;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Extensions
{
    public static class MonoBehaviourExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        static readonly Dictionary<MonoBehaviour, MotionHandle> delayHandles = new();

        [MethodImpl(INLINE)]
        public static void Delay(this MonoBehaviour behaviour, float delay, Action action)
        {
            // Cancel any existing delay for this behaviour
            if (delayHandles.TryGetValue(behaviour, out var existingHandle))
            {
                existingHandle.Cancel();
                delayHandles.Remove(behaviour);
            }

            var handle = LMotion.Create(0f, 1f, delay)
                .WithOnComplete(() =>
                {
                    action?.Invoke();
                    delayHandles.Remove(behaviour);
                })
                .RunWithoutBinding();

            delayHandles[behaviour] = handle;
        }
    }
}