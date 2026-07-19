using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class RigidbodyMovementStrategy : IMovementStrategy
    {
        public void Move(IPhysicsEntity entity, Vector3 direction, float speed, float deltaTime)
        {
            var targetVelocity = new Vector3(direction.x * speed, entity.Velocity.y, direction.z * speed);
            entity.SetVelocity(targetVelocity);
        }
    }
}