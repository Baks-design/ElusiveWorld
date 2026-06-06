using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class CharacterControllerPhysicsEntity : IPhysicsEntity
    {
        readonly CharacterController controller;
        readonly Transform transform;
        Vector3 velocity;
        float mass = 80f;

        public Transform Transform => transform;
        public Vector3 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        public bool IsGrounded => controller.isGrounded;
        public float Mass
        {
            get => mass;
            set => mass = value;
        }

        public CharacterControllerPhysicsEntity(CharacterController controller, Transform transform)
        {
            this.controller = controller;
            this.transform = transform;
        }

        public void ApplyForce(Vector3 force, ForceMode mode)
        {
            var acceleration = force / mass;
            switch (mode)
            {
                case ForceMode.Force: velocity += acceleration * Time.deltaTime; break;
                case ForceMode.Acceleration: velocity += force * Time.deltaTime; break;
                case ForceMode.Impulse: velocity += force / mass; break;
                case ForceMode.VelocityChange: velocity += force; break;
            }
        }

        public void ApplyImpulse(Vector3 impulse) => velocity += impulse / mass;

        public void SetVelocity(Vector3 velocity) => this.velocity = velocity;

        public void MovePosition(Vector3 position)
        {
            var delta = position - transform.position;
            controller.Move(delta);
        }

        public void MoveRotation(Quaternion rotation) => transform.rotation = rotation;

        public void UpdatePhysics()
        {
            if (!controller.isGrounded) velocity.y += UnityEngine.Physics.gravity.y * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}