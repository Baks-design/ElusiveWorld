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

        public void UpdateProcess(InputManager input)
        {
            SmoothInput(input);
            SmoothSpeed();
            SmoothDir();
            CalculateMovementDirection();
            CalculateSpeed(input);
            CalculateFinalMovement();
        }

        void SmoothInput(InputManager input) => flags.smoothInputVector = flags.smoothInputVector.ExpDecay(
            input.MovementAxis, settings.smoothInputSpeed, Time.deltaTime);

        void SmoothSpeed()
        {
            flags.smoothCurrentSpeed = flags.smoothCurrentSpeed.ExpDecay(
                flags.currentSpeed, settings.smoothVelocitySpeed, Time.deltaTime);

            if (flags.isRunning && checks.CanRun() && !flags.isSliding)
            {
                var walkRunPercent = Mathf.InverseLerp(settings.walkSpeed, settings.runSpeed, flags.smoothCurrentSpeed);

                flags.finalSmoothCurrentSpeed =
                    settings.runTransitionCurve.Evaluate(walkRunPercent)
                    * flags.walkRunSpeedDifference + settings.walkSpeed;

                return;
            }

            flags.finalSmoothCurrentSpeed = flags.smoothCurrentSpeed;
        }

        void SmoothDir() => flags.smoothFinalMoveDir = flags.smoothFinalMoveDir.ExpDecay(
            flags.finalMoveDir, settings.smoothFinalDirectionSpeed, Time.deltaTime);

        void CalculateMovementDirection()
        {
            var vDir = controller.transform.forward * flags.smoothInputVector.y;
            var hDir = controller.transform.right * flags.smoothInputVector.x;
            var desiredDir = vDir + hDir;
            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            flags.finalMoveDir = flattenDir;
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
            => flags.isGrounded ? Vector3.ProjectOnPlane(vectorToFlat, flags.hitInfo.normal) : Vector3.zero;

        void CalculateSpeed(InputManager input)
        {
            flags.currentSpeed = flags.isRunning && checks.CanRun() ? settings.runSpeed : settings.walkSpeed;
            flags.currentSpeed = flags.isCrouching ? settings.crouchSpeed : flags.currentSpeed;
            flags.currentSpeed = flags.isSliding ? settings.slideSpeed : flags.currentSpeed;
            flags.currentSpeed = input.MovementAxis == Vector2.zero ? 0f : flags.currentSpeed;
            flags.currentSpeed = input.MovementAxis.y == -1f
                ? flags.currentSpeed * settings.moveBackwardsSpeedPercent : flags.currentSpeed;
            flags.currentSpeed = input.MovementAxis.x != 0f
                && input.MovementAxis.y == 0f ? flags.currentSpeed * settings.moveSideSpeedPercent : flags.currentSpeed;
        }

        void CalculateFinalMovement()
        {
            var finalVector = flags.smoothFinalMoveDir * flags.finalSmoothCurrentSpeed;
            if (controller.isGrounded) flags.finalMoveVector.y += finalVector.y;
            flags.finalMoveVector.x = finalVector.x;
            flags.finalMoveVector.z = finalVector.z;
        }

        public void HandleJump()
        {
            if (!checks.CanJump()) return;

            flags.finalMoveVector.y = settings.jumpSpeed;
            flags.previouslyGrounded = true;
            flags.isGrounded = false;
        }

        public void UpdateVelocity()
        {
            ApplyGravity();
            ApplyMovement();
        }

        void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                flags.inAirTimer = 0f;
                flags.finalMoveVector.y = Mathf.Clamp(
                    flags.finalMoveVector.y -= settings.stickToGroundForce * Time.deltaTime,
                    -settings.stickToGroundForce, settings.jumpSpeed);
            }
            else
            {
                flags.inAirTimer += Time.deltaTime;
                flags.finalMoveVector += settings.gravityMultiplier * Time.deltaTime * Physics.gravity;
            }
        }

        void ApplyMovement() => controller.Move(flags.finalMoveVector * Time.deltaTime);

        public void RotateTowardsCamera() =>
            controller.transform.rotation = controller.transform.rotation.ExpDecay(
                yawTransform.rotation, settings.smoothRotateSpeed, Time.deltaTime);
    }
}