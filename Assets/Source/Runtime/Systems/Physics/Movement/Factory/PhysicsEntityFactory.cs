using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class RigidbodyEntityFactory : IPhysicsEntityFactory
    {
        readonly RaycastConfig defaultRaycastConfig;

        public RigidbodyEntityFactory(RaycastConfig raycastConfig = null) =>
            defaultRaycastConfig = raycastConfig ?? new RaycastConfig();

        public IPhysicsEntity CreateEntity(GameObject target)
        {
            if (!target.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                Debug.LogError($"No Rigidbody found on {target.name}");
                return null;
            }

            return new RigidbodyPhysicsEntity(rigidbody, target.transform);
        }

        public bool CanCreateEntity(GameObject target) => target.TryGetComponent<Rigidbody>(out _);

        public IMovementStrategy CreateMovementStrategy() => new RigidbodyMovementStrategy();

        public IJumpHandler CreateJumpHandler() => new RigidbodyJumpHandler();

        public IRaycastService CreateRaycastService(IPhysicsEntity entity) =>
            new RigidbodyRaycastService(entity, defaultRaycastConfig);
    }
}