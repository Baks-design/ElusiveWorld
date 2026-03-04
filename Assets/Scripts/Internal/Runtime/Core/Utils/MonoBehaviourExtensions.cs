using System;
using UnityEngine;
using LitMotion;
using System.Collections.Generic;

namespace Assets.Scripts.Internal.Runtime.Core.Utils
{
    public static class MonoBehaviourExtensions
    {
        static readonly Dictionary<MonoBehaviour, MotionHandle> delayHandles = new();

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