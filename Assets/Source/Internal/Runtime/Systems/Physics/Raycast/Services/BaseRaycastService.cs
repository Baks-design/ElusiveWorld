using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public abstract class BaseRaycastService : IRaycastService
    {
        protected readonly IPhysicsEntity Entity;
        protected readonly RaycastConfig Config;
        protected readonly Transform Transform;

        public bool IsOnSlope { get; protected set; }
        public float SlopeAngle { get; protected set; }
        public Vector3 GroundNormal { get; protected set; } = Vector3.up;
        public Collider GroundCollider { get; protected set; }

        protected BaseRaycastService(IPhysicsEntity entity, RaycastConfig config)
        {
            Entity = entity;
            Config = config;
            Transform = entity.Transform;
        }

        public abstract RaycastResult CheckGround(float maxDistance = 0.1f);
        public abstract RaycastResult CheckCeiling(float maxDistance = 0.1f);
        public abstract RaycastResult CheckWall(Vector3 direction, float maxDistance = 0.1f);
        public abstract RaycastResult CheckSlope(out float slopeAngle);
    }
}