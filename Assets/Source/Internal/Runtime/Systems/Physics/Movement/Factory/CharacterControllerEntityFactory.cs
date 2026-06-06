using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class CharacterControllerEntityFactory : IPhysicsEntityFactory
    {
        readonly RaycastConfig defaultRaycastConfig;

        public CharacterControllerEntityFactory(RaycastConfig raycastConfig = null) =>
            defaultRaycastConfig = raycastConfig ?? new RaycastConfig();

        public IPhysicsEntity CreateEntity(GameObject target)
        {
            if (!target.TryGetComponent<CharacterController>(out var controller))
            {
                Debug.LogError($"No CharacterController found on {target.name}");
                return null;
            }

            return new CharacterControllerPhysicsEntity(controller, target.transform);
        }

        public bool CanCreateEntity(GameObject target) => target.TryGetComponent<CharacterController>(out _);

        public IMovementStrategy CreateMovementStrategy() => new CharacterControllerMovementStrategy();

        public IJumpHandler CreateJumpHandler() => new CharacterControllerJumpHandler();

        public IRaycastService CreateRaycastService(IPhysicsEntity entity) =>
            new CharacterControllerRaycastService(entity, defaultRaycastConfig);
    }
}