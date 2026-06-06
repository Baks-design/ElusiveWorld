using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public interface IPhysicsEntity
    {
        Transform Transform { get; }
        Vector3 Velocity { get; set; }
        bool IsGrounded { get; }
        float Mass { get; }

        void ApplyForce(Vector3 force, ForceMode mode);
        void ApplyImpulse(Vector3 impulse);
        void SetVelocity(Vector3 velocity);
        void MovePosition(Vector3 position);
        void MoveRotation(Quaternion rotation);
    }
}