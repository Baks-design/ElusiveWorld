using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersChecks
    {
        readonly CharactersFlags flags;
        readonly MovementSettings settings;
        readonly CharacterController controller;
        readonly InputManager input;

        public CharactersChecks(
            CharactersFlags flags,
            MovementSettings settings,
            CharacterController controller,
            InputManager input)
        {
            this.flags = flags;
            this.settings = settings;
            this.controller = controller;
            this.input = input;

            InitializeSettings();
        }

        void InitializeSettings()
        {
            controller.center = new(0f, controller.height / 2f + controller.skinWidth, 0f);
            flags.isGrounded = true;
            flags.previouslyGrounded = true;
            flags.finalRayLength = settings.rayLength + controller.center.y;
        }

        public void Update()
        {
            CheckGrounded();
            CheckWall();
        }

        void CheckGrounded()
        {
            var origin = controller.transform.position + controller.center;

            flags.isGrounded = Physics.SphereCast(
                origin,
                settings.raySphereRadius,
                Vector3.down,
                out flags.hitInfo,
                flags.finalRayLength,
                settings.groundLayer);
        }

        void CheckWall()
        {
            if (input.MovementAxis == Vector2.zero || flags.finalMoveDir.sqrMagnitude <= 0f)
            {
                flags.isHitWall = false;
                return;
            }

            var origin = controller.transform.position + controller.center;

            flags.isHitWall = Physics.SphereCast(
                origin,
                settings.rayObstacleSphereRadius,
                flags.finalMoveDir,
                out _,
                settings.rayObstacleLength,
                settings.obstacleLayers);
        }

        public bool CheckIfRoof() =>
            Physics.SphereCast(
                controller.transform.position,
                settings.raySphereRadius,
                Vector3.up,
                out _,
                flags.initHeight);

        public bool CanRun()
        {
            if (flags.isCrouching || flags.smoothFinalMoveDir == Vector3.zero) return false;

            var dot = Vector3.Dot(controller.transform.forward, flags.smoothFinalMoveDir.normalized);
            return dot >= settings.canRunThreshold;
        }

        public bool CanJump() => !flags.isSliding && !flags.isCrouching && controller.isGrounded;

        public bool CanCrouch() => controller.isGrounded;
    }
}