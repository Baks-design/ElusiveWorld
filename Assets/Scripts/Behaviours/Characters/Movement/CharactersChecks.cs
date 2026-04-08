using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersChecks
    {
        readonly CharactersFlags flags;
        readonly MovementSettings settings;
        readonly CharacterController controller;

        public CharactersChecks(
            CharactersFlags flags,
            MovementSettings settings,
            CharacterController controller)
        {
            this.flags = flags;
            this.settings = settings;
            this.controller = controller;

            flags.isGrounded = true;
            flags.previouslyGrounded = true;
            flags.finalRayLength = settings.rayLength + controller.center.y;
        }

        public void Update(InputManager input)
        {
            CheckIfGrounded();
            CheckIfWall(input);
        }

        void CheckIfGrounded()
        {
            var origin = controller.transform.position + controller.center;
            flags.isGrounded = Physics.SphereCast(
                origin, settings.raySphereRadius, Vector3.down,
                out flags.hitInfo, flags.finalRayLength, settings.groundLayer);
        }

        void CheckIfWall(InputManager input)
        {
            var origin = controller.transform.position + controller.center;
            var hitWall = false;
            if (input.MovementAxis != Vector2.zero && flags.finalMoveDir.sqrMagnitude > 0f)
                hitWall = Physics.SphereCast(
                    origin, settings.rayObstacleSphereRadius, flags.finalMoveDir,
                    out var _, settings.rayObstacleLength, settings.obstacleLayers);
            flags.isHitWall = hitWall;
        }

        public bool CheckIfRoof() =>
            Physics.SphereCast(
                controller.transform.position,
                settings.raySphereRadius,
                Vector3.up,
                out var _,
                flags.initHeight);

        public bool CanRun()
        {
            var normalizedDir = Vector3.zero;
            if (flags.smoothFinalMoveDir != Vector3.zero) normalizedDir = flags.smoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(controller.transform.forward, normalizedDir);
            return dot >= settings.canRunThreshold && !flags.isCrouching;
        }

        public bool CanJump() => !flags.isSliding && !flags.isCrouching && controller.isGrounded;

        public bool CanCrouch() => controller.isGrounded;
    }
}