using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class RigidbodyPhysicsEntity : IPhysicsEntity
    {
        readonly Rigidbody rigidbody;
        readonly Transform transform;

        public Transform Transform => transform;
        public Vector3 Velocity
        {
            get => rigidbody.linearVelocity;
            set => rigidbody.linearVelocity = value;
        }
        public bool IsGrounded
        {
            get
            {
                var rayLength = 1.1f;
                return UnityEngine.Physics.Raycast(transform.position, Vector3.down, rayLength);
            }
        }
        public float Mass => rigidbody.mass;

        public RigidbodyPhysicsEntity(Rigidbody rigidbody, Transform transform)
        {
            this.rigidbody = rigidbody;
            this.transform = transform;
        }

        public void ApplyForce(Vector3 force, ForceMode mode) => rigidbody.AddForce(force, mode);

        public void ApplyImpulse(Vector3 impulse) => rigidbody.AddForce(impulse, ForceMode.Impulse);

        public void SetVelocity(Vector3 velocity) => rigidbody.linearVelocity = velocity;

        public void MovePosition(Vector3 position) => rigidbody.MovePosition(position);

        public void MoveRotation(Quaternion rotation) => rigidbody.MoveRotation(rotation);
    }
}