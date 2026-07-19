using System.Runtime.CompilerServices;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Extensions
{
    public static class ColliderExtensions
    {
        const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        static readonly Collider[] overlapCache = new Collider[32];

        [MethodImpl(INLINE)]
        public static bool GetPenetrationsInLayer(
            this Collider source,
            LayerMask layerMask,
            out Vector3 totalCorrection)
        {
            totalCorrection = Vector3.zero;

            if (source == null) return false;

            var bounds = source.bounds;
            var count = Physics.OverlapBoxNonAlloc(
                bounds.center,
                bounds.extents,
                overlapCache,
                source.transform.rotation,
                layerMask,
                QueryTriggerInteraction.Ignore
            );
            if (count == overlapCache.Length)
            {
                // Optional: buffer overflow warning
                // Debug.LogWarning("Overlap buffer full, some collisions may be missed.");
            }

            var collided = false;
            var maxDistance = 0f;
            for (var i = 0; i < count; i++)
            {
                var other = overlapCache[i];
                if (other == null || other == source) continue;

                if (Physics.ComputePenetration(
                    source, source.transform.position, source.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out var dir, out var dist))
                {
                    collided = true;

                    // Use max penetration (more stable than summing)
                    if (dist > maxDistance)
                    {
                        maxDistance = dist;
                        totalCorrection = dir * dist;
                    }
                }
            }

            return collided;
        }

        [MethodImpl(INLINE)]
        public static bool ComputePenetration(
            this Collider source,
            Collider target,
            out Vector3 direction,
            out float distance)
        {
            direction = Vector3.zero;
            distance = 0f;

            if (source == null || target == null)  return false;

            return Physics.ComputePenetration(
                source, source.transform.position, source.transform.rotation,
                target, target.transform.position, target.transform.rotation,
                out direction, out distance
            );
        }
    }
}