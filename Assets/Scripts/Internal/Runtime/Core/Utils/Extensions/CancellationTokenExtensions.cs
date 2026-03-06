using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static CancellationToken CombineWithDestroyToken(
            this CancellationToken token, MonoBehaviour behaviour) =>
                CancellationTokenSource.CreateLinkedTokenSource(
                    token, behaviour.GetCancellationTokenOnDestroy()).Token;
    }
}