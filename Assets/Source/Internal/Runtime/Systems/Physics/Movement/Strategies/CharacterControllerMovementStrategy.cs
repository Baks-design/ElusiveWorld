using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class CharacterControllerMovementStrategy : IMovementStrategy
    {
        public void Move(IPhysicsEntity entity, Vector3 direction, float speed, float deltaTime)
        {
            var movement = new Vector3(
                direction.x * speed * deltaTime,
                entity.Velocity.y * deltaTime,
                direction.z * speed * deltaTime);
            entity.MovePosition(entity.Transform.position + movement);
        }
    }
}