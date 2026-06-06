using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class CharacterControllerJumpHandler : IJumpHandler
    {
        public void Jump(IPhysicsEntity entity, float jumpForce)
        {
            if (!entity.IsGrounded) return;

            var velocity = entity.Velocity;
            velocity.y = Mathf.Sqrt(2f * Mathf.Abs(UnityEngine.Physics.gravity.y) * jumpForce);
            entity.SetVelocity(velocity);
        }
    }
}