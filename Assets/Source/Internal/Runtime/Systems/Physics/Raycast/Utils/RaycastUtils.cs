using System.Collections.Generic;
using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public static class RaycastUtils
    {
        public static List<RaycastResult> ConeCast(Vector3 origin, Vector3 direction, float maxDistance,
            float coneAngle, int rayCount, LayerMask layerMask)
        {
            var results = new List<RaycastResult>();
            for (var i = 0; i < rayCount; i++)
            {
                var angle = (i / (float)(rayCount - 1) - 0.5f) * coneAngle;
                var rotation = Quaternion.AngleAxis(angle, Vector3.up);
                var rayDirection = rotation * direction;
                if (UnityEngine.Physics.Raycast(origin, rayDirection, out var hit, maxDistance, layerMask))
                    results.Add(new RaycastResult
                    {
                        Hit = true,
                        Point = hit.point,
                        Normal = hit.normal,
                        Distance = hit.distance,
                        Collider = hit.collider,
                        HitTransform = hit.transform
                    });
            }
            return results;
        }

        /// <summary>
        /// Check for obstacles between two points
        /// </summary>
        public static bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask obstacleMask, float radius = 0.1f)
        {
            var direction = to - from;
            var distance = direction.magnitude;
            return radius switch
            {
                > 0f => !UnityEngine.Physics.SphereCast(from, radius, direction.normalized, out _, distance, obstacleMask),
                _ => !UnityEngine.Physics.Raycast(from, direction.normalized, distance, obstacleMask),
            };
        }

        /// <summary>
        /// Find the closest point on the ground
        /// </summary>
        public static Vector3? GetGroundPoint(Vector3 position, float maxDistance = 10f, LayerMask groundMask = default)
        {
            if (UnityEngine.Physics.Raycast(position, Vector3.down, out var hit, maxDistance, groundMask))
                return hit.point;
            return null;
        }

        /// <summary>
        /// Check if a point is walkable based on slope angle
        /// </summary>
        public static bool IsWalkable(Vector3 position, Vector3 direction, float maxSlopeAngle,
            float checkDistance = 1f, LayerMask groundMask = default)
        {
            if (UnityEngine.Physics.Raycast(position + Vector3.up * 0.1f, direction, out var hit,
                checkDistance, groundMask))
            {
                var angle = Vector3.Angle(hit.normal, Vector3.up);
                return angle <= maxSlopeAngle;
            }
            return false;
        }
    }
}