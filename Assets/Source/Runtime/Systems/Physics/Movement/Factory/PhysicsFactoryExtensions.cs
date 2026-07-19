using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public static class PhysicsFactoryExtensions
    {
        /// <summary>
        /// Extension method to easily create physics service from any GameObject
        /// </summary>
        public static PhysicsService CreatePhysicsService(this GameObject gameObject, RaycastConfig config = null) =>
            PhysicsFactoryRegistry.Instance.CreateService(gameObject, config);

        /// <summary>
        /// Try to create physics service, returns null if no compatible component found
        /// </summary>
        public static bool TryCreatePhysicsService(this GameObject gameObject, out PhysicsService service,
            RaycastConfig config = null)
        {
            service = CreatePhysicsService(gameObject, config);
            return service != null;
        }
    }
}