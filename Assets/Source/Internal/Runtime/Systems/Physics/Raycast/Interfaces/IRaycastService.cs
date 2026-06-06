using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public interface IRaycastService
    {
        bool IsOnSlope { get; }
        float SlopeAngle { get; }
        Vector3 GroundNormal { get; }
        Collider GroundCollider { get; }

        RaycastResult CheckGround(float maxDistance = 0.1f);
        RaycastResult CheckCeiling(float maxDistance = 0.1f);
        RaycastResult CheckWall(Vector3 direction, float maxDistance = 0.1f);
        RaycastResult CheckSlope(out float slopeAngle);
    }
}