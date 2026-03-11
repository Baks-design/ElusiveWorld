using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using ZLinq;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress => Handles.Count == 0f
            ? 0f : Handles.AsValueEnumerable().Average(h => h.PercentComplete);
        public bool IsDone => Handles.Count == 0 || Handles.AsValueEnumerable().All(h => h.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity)
            => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);

        // Helper method to await all handles
        public async UniTask WhenAll(CancellationToken cancellationToken = default)
        {
            if (Handles.Count == 0) return;

            await UniTask.WhenAll(Handles.Select(h => h.ToUniTask(PlayerLoopTiming.Update, cancellationToken)));
        }

        // Helper method to get all loaded scenes
        public List<string> GetLoadedSceneNames()
            => Handles
                .AsValueEnumerable()
                .Where(h => h.IsValid() && h.Result.Scene.isLoaded)
                .Select(h => h.Result.Scene.name)
                .ToList();
    }
}