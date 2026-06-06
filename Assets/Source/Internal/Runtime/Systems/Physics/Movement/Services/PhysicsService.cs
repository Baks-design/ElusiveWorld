using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class PhysicsService
    {
        readonly IPhysicsEntity entity;
        readonly IMovementStrategy movementStrategy;
        readonly IJumpHandler jumpHandler;
        readonly IRaycastService raycastService;
        readonly float slopeSpeedMultiplier = 1f;
        readonly bool preventSlopeSlide = true;

        public IRaycastService Raycast => raycastService;
        public bool IsGrounded => entity.IsGrounded;
        public bool IsOnSlope => raycastService.IsOnSlope;
        public float CurrentSlopeAngle => raycastService.SlopeAngle;
        public Vector2 Velocity
        {
            get => entity.Velocity;
            set => entity.Velocity = value;
        }

        public PhysicsService(IPhysicsEntity entity, IMovementStrategy movementStrategy,
            IJumpHandler jumpHandler, IRaycastService raycastService)
        {
            this.entity = entity;
            this.movementStrategy = movementStrategy;
            this.jumpHandler = jumpHandler;
            this.raycastService = raycastService;
        }

        public void Move(Vector3 direction, float speed)
        {
            if (raycastService.IsOnSlope)
            {
                direction = AdjustDirectionToSlope(direction);
                speed *= slopeSpeedMultiplier;
            }

            movementStrategy.Move(entity, direction, speed, Time.deltaTime);
        }

        public void Jump(float jumpForce)
        {
            if (!entity.IsGrounded)
            {
                // Check for coyote time or wall jump here
                CheckWallJumpOpportunity();
                return;
            }

            jumpHandler.Jump(entity, jumpForce);
        }

        public void ApplyGravity(float gravityMultiplier = 1f)
        {
            var gravity = UnityEngine.Physics.gravity * gravityMultiplier;

            if (raycastService.IsOnSlope && preventSlopeSlide)
            {
                // Project gravity onto slope to prevent sliding
                var slopeNormal = raycastService.GroundNormal;
                gravity = Vector3.ProjectOnPlane(gravity, slopeNormal);
            }

            entity.ApplyForce(gravity * entity.Mass, ForceMode.Force);
        }

        public bool CanStandUp() => !raycastService.CheckCeiling().Hit;

        public bool CanMoveInDirection(Vector3 direction, float distance) =>
            !raycastService.CheckWall(direction, distance).Hit;

        public bool TryStepUp(out Vector3 stepPosition)
        {
            stepPosition = entity.Transform.position;

            // Check for step obstacle
            var forward = entity.Transform.forward;
            var wallCheck = raycastService.CheckWall(forward, 0.3f);
            if (!wallCheck.Hit) return false;

            // Check if step is climbable
            var stepCheckOrigin = entity.Transform.position + forward * 0.3f + Vector3.up * 0.5f;
            if (UnityEngine.Physics.Raycast(stepCheckOrigin, Vector3.down, out var hit, 0.5f))
            {
                var stepHeight = entity.Transform.position.y - hit.point.y;
                if (stepHeight <= 0.3f && stepHeight > 0.01f)
                {
                    stepPosition = hit.point + Vector3.up * 0.05f;
                    // Check if we can stand at the new position
                    if (!UnityEngine.Physics.CheckSphere(stepPosition + Vector3.up * 0.5f, 0.3f)) return true;
                }
            }

            return false;
        }

        Vector3 AdjustDirectionToSlope(Vector3 direction)
        {
            var slopeNormal = raycastService.GroundNormal;
            return Vector3.ProjectOnPlane(direction, slopeNormal).normalized;
        }

        void CheckWallJumpOpportunity()
        {
            Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            foreach (var dir in directions)
            {
                var wallCheck = raycastService.CheckWall(dir, 0.3f);
                if (wallCheck.Hit) Debug.Log($"Wall detected in direction: {dir}");
            }
        }
    }
}