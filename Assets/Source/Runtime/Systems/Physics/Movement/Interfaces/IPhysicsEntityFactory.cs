using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public interface IPhysicsEntityFactory
    {
        IPhysicsEntity CreateEntity(GameObject target);
        bool CanCreateEntity(GameObject target);
    }
}