using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class AddressableExtensions
    {
        public static UniTask ToUniTask(
            this AsyncOperationHandle<SceneInstance> handle, CancellationToken cancellationToken = default)
        {
            if (!handle.IsValid() || handle.IsDone)
                return UniTask.CompletedTask;

            var tcs = new UniTaskCompletionSource();

            void Callback(AsyncOperationHandle<SceneInstance> op)
            {
                handle.Completed -= Callback;
                if (op.Status == AsyncOperationStatus.Succeeded)
                    tcs.TrySetResult();
                else
                    tcs.TrySetException(new Exception($"Failed to load scene: {op.OperationException}"));
            }

            handle.Completed += Callback;

            return tcs.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}