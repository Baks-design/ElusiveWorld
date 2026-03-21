using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress
        {
            get
            {
                if (Operations.Count == 0) return 0;
                var total = 0f;
                for (var i = 0; i < Operations.Count; i++)
                    total += Operations[i].progress;
                return total / Operations.Count;
            }
        }
        public bool IsDone
        {
            get
            {
                for (var i = 0; i < Operations.Count; i++)
                    if (!Operations[i].isDone)
                        return false;
                return true;
            }
        }

        public AsyncOperationGroup(int initialCapacity)
            => Operations = new List<AsyncOperation>(initialCapacity);
    }

    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress
        {
            get
            {
                if (Handles.Count == 0) return 0;
                var total = 0f;
                for (var i = 0; i < Handles.Count; i++)
                    total += Handles[i].PercentComplete;
                return total / Handles.Count;
            }
        }

        public bool IsDone
        {
            get
            {
                if (Handles.Count == 0)
                    for (var i = 0; i < Handles.Count; i++)
                        if (!Handles[i].IsDone) return false;
                return true;
            }
        }

        public AsyncOperationHandleGroup(int initialCapacity)
            => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}