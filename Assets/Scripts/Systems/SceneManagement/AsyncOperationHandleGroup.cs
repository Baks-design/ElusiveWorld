using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using ZLinq;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.AsValueEnumerable().Average(o => o.progress);
        public bool IsDone => Operations.AsValueEnumerable().All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
            => Operations = new List<AsyncOperation>(initialCapacity);
    }

    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress => Handles.Count == 0 ? 0 : Handles.AsValueEnumerable().Average(h => h.PercentComplete);
        public bool IsDone => Handles.Count == 0 || Handles.AsValueEnumerable().All(o => o.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity)
            => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}