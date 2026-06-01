using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersChecks
    {
        readonly CharactersFlags flags;
        readonly MovementSettings settings;
        readonly CharacterController controller;
        readonly InputManager input;
        const float EPSILON = 0.0001f;

        public CharactersChecks(
            CharactersFlags flags,
            MovementSettings settings,
            CharacterController controller,
            InputManager input)
        {
            this.flags = flags ?? throw new ArgumentNullException(nameof(flags));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.controller = controller != null ? controller : throw new ArgumentNullException(nameof(controller));
            this.input = input != null ? input : throw new ArgumentNullException(nameof(input));

            InitializeSettings();
        }

        void InitializeSettings()
        {
            flags.isGrounded = true;
            flags.previouslyGrounded = true;
        }

        public void Update()
        {
            flags.previouslyGrounded = flags.isGrounded;
            CheckGrounded();
            CheckWall();
        }

        void CheckGrounded()
        {
            var origin = controller.transform.position + Vector3.up * (controller.radius + 0.01f);
            var distance = settings.rayLength;
            flags.isGrounded = Physics.SphereCast(
                origin,
                controller.radius,
                Vector3.down,
                out flags.hitInfo,
                distance,
                settings.groundLayer
            );
        }

        void CheckWall()
        {
            if (input.MovementAxis == Vector2.zero || flags.finalMoveDir.sqrMagnitude < EPSILON)
            {
                flags.isHitWall = false;
                return;
            }

            var dir = flags.finalMoveDir;
            dir.y = 0f;
            if (dir.sqrMagnitude < EPSILON)
            {
                flags.isHitWall = false;
                return;
            }
            dir.Normalize();

            var origin = controller.transform.position + Vector3.up * (controller.height * 0.5f);
            flags.isHitWall = Physics.SphereCast(
                origin,
                settings.rayObstacleSphereRadius,
                dir,
                out _,
                settings.rayObstacleLength,
                settings.obstacleLayers
            );
        }

        public bool CheckIfRoof()
        {
            var origin = controller.transform.position + Vector3.up * (controller.height - controller.radius);
            return Physics.SphereCast(
                origin,
                controller.radius,
                Vector3.up,
                out _,
                settings.rayLength
            );
        }

        public bool CanRun()
        {
            if (flags.isCrouching || flags.smoothFinalMoveDir.sqrMagnitude < EPSILON) return false;
            var forward = controller.transform.forward;
            var move = flags.smoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(forward, move);
            return dot >= settings.canRunThreshold;
        }

        public bool CanJump() => !flags.isSliding && !flags.isCrouching && flags.isGrounded;

        public bool CanCrouch() => flags.isGrounded;

        public bool CanSlope()
        {
            if (!flags.isGrounded) return false;
            var angle = Vector3.Angle(flags.hitInfo.normal, Vector3.up);
            return angle > controller.slopeLimit;
        }
    }
}