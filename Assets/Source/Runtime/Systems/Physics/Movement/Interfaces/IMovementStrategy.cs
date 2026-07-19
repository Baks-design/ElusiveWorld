using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public interface IMovementStrategy
    {
        void Move(IPhysicsEntity entity, Vector3 direction, float speed, float deltaTime);
    }
}