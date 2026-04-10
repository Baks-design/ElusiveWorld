using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersDisplacement
    {
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly MovementSettings settings;
        readonly CharactersChecks checks;
        readonly CharactersFlags flags;

        public CharactersDisplacement(
            MovementSettings settings,
            CharactersChecks checks,
            CharacterController controller,
            CharactersFlags flags,
            HeadBob headBob,
            Transform yawTransform)
        {
            this.settings = settings;
            this.checks = checks;
            this.controller = controller;
            this.flags = flags;
            this.yawTransform = yawTransform;

            flags.inAirTimer = 0f;
            headBob.CurrentStateHeight = flags.initCamHeight;
            flags.walkRunSpeedDifference = settings.runSpeed - settings.walkSpeed;
        }

        public void UpdateProcess(float dt, InputManager input)
        {
            SmoothInput(dt, input);
            SmoothSpeed(dt);
            SmoothDirection(dt);
            CalculateMovementDirection();
            CalculateSpeed(input);
            CalculateFinalMovement();
        }

        void SmoothInput(float dt, InputManager input) =>
            flags.smoothInputVector = flags.smoothInputVector.ExpDecay(
                input.MovementAxis,
                settings.smoothInputSpeed,
                dt);

        void SmoothSpeed(float dt)
        {
            flags.smoothCurrentSpeed = flags.smoothCurrentSpeed.ExpDecay(
                flags.currentSpeed,
                settings.smoothVelocitySpeed,
                dt);

            if (flags.isRunning && checks.CanRun() && !flags.isSliding)
            {
                var percent = 
                    settings.walkSpeed.InverseEerp(
                    settings.runSpeed,
                    flags.smoothCurrentSpeed);

                var curve = settings.runTransitionCurve.Evaluate(percent);

                flags.finalSmoothCurrentSpeed =
                    curve * flags.walkRunSpeedDifference + settings.walkSpeed;
            }
            else
                flags.finalSmoothCurrentSpeed = flags.smoothCurrentSpeed;
        }

        void SmoothDirection(float dt) =>
            flags.smoothFinalMoveDir = flags.smoothFinalMoveDir.ExpDecay(
                flags.finalMoveDir,
                settings.smoothFinalDirectionSpeed,
                dt);

        void CalculateMovementDirection()
        {
            var forward = controller.transform.forward * flags.smoothInputVector.y;
            var right = controller.transform.right * flags.smoothInputVector.x;

            var desired = forward + right;

            flags.finalMoveDir = flags.isGrounded
                ? Vector3.ProjectOnPlane(desired, flags.hitInfo.normal)
                : desired;
        }

        void CalculateSpeed(InputManager input)
        {
            var speed =
                flags.isSliding ? settings.slideSpeed :
                flags.isCrouching ? settings.crouchSpeed :
                (flags.isRunning && checks.CanRun() ? settings.runSpeed : settings.walkSpeed);

            if (input.MovementAxis == Vector2.zero)
            {
                flags.currentSpeed = 0f;
                return;
            }

            if (input.MovementAxis.y == -1f)
                speed *= settings.moveBackwardsSpeedPercent;
            if (input.MovementAxis.x != 0f && input.MovementAxis.y == 0f)
                speed *= settings.moveSideSpeedPercent;

            flags.currentSpeed = speed;
        }

        void CalculateFinalMovement()
        {
            var final = flags.smoothFinalMoveDir * flags.finalSmoothCurrentSpeed;
            
            flags.finalMoveVector.x = final.x;
            flags.finalMoveVector.z = final.z;
            if (controller.isGrounded)
                flags.finalMoveVector.y += final.y;
        }

        public void HandleJump()
        {
            if (!checks.CanJump()) return;

            flags.finalMoveVector.y = settings.jumpSpeed;
            flags.previouslyGrounded = true;
            flags.isGrounded = false;
        }

        public void UpdateVelocity(float dt)
        {
            ApplyGravity(dt);
            controller.Move(flags.finalMoveVector * dt);
        }

        void ApplyGravity(float dt)
        {
            if (controller.isGrounded)
            {
                flags.inAirTimer = 0f;

                flags.finalMoveVector.y = Mathf.Clamp(
                    flags.finalMoveVector.y - settings.stickToGroundForce * dt,
                    -settings.stickToGroundForce,
                    settings.jumpSpeed);
            }
            else
            {
                flags.inAirTimer += dt;

                flags.finalMoveVector += settings.gravityMultiplier * dt * Physics.gravity;
            }
        }

        public void RotateTowardsCamera(float dt)
        => controller.transform.rotation =
            controller.transform.rotation.ExpDecay(
            yawTransform.rotation,
            settings.smoothRotateSpeed,
            dt);
    }
}