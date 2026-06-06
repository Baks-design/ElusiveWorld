using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class RigidbodyJumpHandler : IJumpHandler
    {
        public void Jump(IPhysicsEntity entity, float jumpForce)
        {
            if (!entity.IsGrounded) return;

            var jumpVelocity = entity.Velocity;
            jumpVelocity.y = Mathf.Sqrt(2f * Mathf.Abs(UnityEngine.Physics.gravity.y) * jumpForce);
            entity.SetVelocity(jumpVelocity);
        }
    }
}