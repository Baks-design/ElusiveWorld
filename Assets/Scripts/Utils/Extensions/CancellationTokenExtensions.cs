using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class CancellationTokenExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static CancellationToken CombineWithDestroyToken(
            this CancellationToken token, MonoBehaviour behaviour) =>
                CancellationTokenSource.CreateLinkedTokenSource(
                    token, behaviour.GetCancellationTokenOnDestroy()).Token;
    }
}