using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using System;
using ElusiveWorld.Internal.Runtime.Systems.Physics;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersChecks
    {
        readonly CharactersFlags flags;
        readonly MovementSettings settings;
        readonly CharacterController controller;
        readonly InputManager input;
        readonly PhysicsService physicsService;
        const float EPSILON = 0.0001f;

        public CharactersChecks(CharactersFlags flags, MovementSettings settings, CharacterController controller,
            InputManager input, PhysicsService physicsService)
        {
            this.flags = flags ?? throw new ArgumentNullException(nameof(flags));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.controller = controller != null ? controller : throw new ArgumentNullException(nameof(controller));
            this.input = input != null ? input : throw new ArgumentNullException(nameof(input));
            this.physicsService = physicsService ?? throw new ArgumentNullException(nameof(physicsService));

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

        void CheckGrounded() => flags.isGrounded = physicsService.Raycast.CheckGround(settings.rayLength).Hit;

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

            flags.isHitWall = physicsService.Raycast.CheckWall(dir, settings.rayObstacleLength).Hit;
        }

        public bool CheckIfRoof() => physicsService.Raycast.CheckCeiling(settings.rayObstacleLength).Hit;

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