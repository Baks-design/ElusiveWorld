using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class AwaitableExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        [MethodImpl(INLINE)]
        public static Awaitable WaitUntil(
            this Func<bool> condition,
            int pollIntervalMs = 33,
            CancellationToken cancellationToken = default)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (pollIntervalMs <= 0) throw new ArgumentOutOfRangeException(nameof(pollIntervalMs));

            var source = new AwaitableCompletionSource();

            try
            {
                if (condition())
                {
                    source.SetResult();
                    return source.Awaitable;
                }
            }
            catch (Exception ex)
            {
                source.SetException(ex);
                return source.Awaitable;
            }

            _ = PollAsync(source, condition, pollIntervalMs, cancellationToken);

            return source.Awaitable;
        }

        static async Awaitable PollAsync(
            AwaitableCompletionSource source,
            Func<bool> condition,
            int intervalMs,
            CancellationToken ct)
        {
            try
            {
                var seconds = intervalMs / 1000f;

                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        source.SetCanceled();
                        return;
                    }

                    if (condition())
                    {
                        source.SetResult();
                        return;
                    }

                    await Awaitable.WaitForSecondsAsync(seconds);
                }
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
        }
    }
}