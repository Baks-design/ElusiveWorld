using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersDisplacement
    {
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly InputManager input;
        readonly MovementSettings settings;
        readonly HeadBob headBob;
        readonly CharactersChecks checks;
        readonly CharactersFlags flags;

        public CharactersDisplacement(
            MovementSettings settings,
            CharactersChecks checks,
            CharacterController controller,
            CharactersFlags flags,
            HeadBob headBob,
            Transform yawTransform,
            InputManager input)
        {
            this.settings = settings;
            this.checks = checks;
            this.controller = controller;
            this.flags = flags;
            this.headBob = headBob;
            this.yawTransform = yawTransform;
            this.input = input;

            InitializeSettings();
        }

        void InitializeSettings()
        {
            flags.inAirTimer = 0f;
            headBob.CurrentStateHeight = flags.initCamHeight;
            flags.walkRunSpeedDifference = settings.runSpeed - settings.walkSpeed;
        }

        public void RotateTowardsCamera() =>
            controller.transform.rotation = controller.transform.rotation.ExpDecay(
                yawTransform.rotation,
                settings.smoothRotateSpeed,
                Time.deltaTime);

        public void UpdateProcess()
        {
            SmoothInput();
            SmoothSpeed();
            CalculateMovementDirection();
            SmoothDirection();
            CalculateSpeed();
            CalculateFinalMovement();
        }

        void SmoothInput() =>
            flags.smoothInputVector = flags.smoothInputVector.ExpDecay(
                input.MovementAxis,
                settings.smoothInputSpeed,
                Time.deltaTime);

        void SmoothSpeed()
        {
            flags.smoothCurrentSpeed = flags.smoothCurrentSpeed.ExpDecay(
                flags.currentSpeed,
                settings.smoothVelocitySpeed,
                Time.deltaTime);

            if (flags.isRunning && checks.CanRun() && !flags.isSliding)
            {
                var percent = Mathf.InverseLerp(
                    settings.walkSpeed,
                    settings.runSpeed,
                    flags.smoothCurrentSpeed);

                var curve = settings.runTransitionCurve.Evaluate(percent);

                flags.finalSmoothCurrentSpeed =
                    curve * flags.walkRunSpeedDifference + settings.walkSpeed;
            }
            else
                flags.finalSmoothCurrentSpeed = flags.smoothCurrentSpeed;
        }

        void SmoothDirection() =>
            flags.smoothFinalMoveDir = flags.smoothFinalMoveDir.ExpDecay(
                flags.finalMoveDir,
                settings.smoothFinalDirectionSpeed,
                Time.deltaTime);

        void CalculateMovementDirection()
        {
            var forward = yawTransform.forward * flags.smoothInputVector.y;
            var right = yawTransform.right * flags.smoothInputVector.x;
            var desired = Vector3.ClampMagnitude(forward + right, 1f);

            flags.finalMoveDir = controller.isGrounded
                ? Vector3.ProjectOnPlane(desired, flags.hitInfo.normal)
                : flags.finalMoveDir.ExpDecay(desired, settings.airControl, Time.deltaTime);
        }

        void CalculateSpeed()
        {
            var speed =
                flags.isSliding ? settings.slideSpeed :
                flags.isCrouching ? settings.crouchSpeed :
                (flags.isRunning && checks.CanRun() ? settings.runSpeed : settings.walkSpeed);

            if (flags.smoothInputVector.sqrMagnitude < 0.01f)
            {
                flags.currentSpeed = 0f;
                return;
            }

            if (input.MovementAxis.y < -0.1f)
                speed *= settings.moveBackwardsSpeedPercent;
            if (Mathf.Abs(input.MovementAxis.x) > 0.1f && Mathf.Abs(input.MovementAxis.y) < 0.1f)
                speed *= settings.moveSideSpeedPercent;

            flags.currentSpeed = speed;
        }

        void CalculateFinalMovement()
        {
            var final = flags.smoothFinalMoveDir * flags.finalSmoothCurrentSpeed;
            flags.horizontalVelocity.x = final.x;
            flags.horizontalVelocity.z = final.z;
        }

        public void HandleJump()
        {
            if (!checks.CanJump()) return;

            if (flags.verticalVelocity < 0f) flags.verticalVelocity = 0f;
            flags.verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * settings.jumpHeight);
        }

        public void UpdateVelocity()
        {
            ApplyGravity();
            HandleSlopeSliding();
            ApplyMove();
        }

        void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                flags.inAirTimer = 0f;
                if (flags.verticalVelocity < 0f)
                    flags.verticalVelocity = -settings.stickToGroundForce;
            }
            else
            {
                flags.inAirTimer += Time.deltaTime;
                flags.verticalVelocity += Physics.gravity.y * settings.gravityMultiplier * Time.deltaTime;
            }
        }

        void HandleSlopeSliding()
        {
            if (!checks.CanSlope())
            {
                flags.slideVelocity = Vector3.zero;
                return;
            }

            var slopeAngle = Vector3.Angle(flags.hitInfo.normal, Vector3.up);
            if (slopeAngle <= controller.slopeLimit)
            {
                flags.slideVelocity = flags.slideVelocity.ExpDecay(Vector3.zero, settings.slopeAcceleration, Time.deltaTime);
                return;
            }

            var slopeDir = Vector3.ProjectOnPlane(Vector3.down, flags.hitInfo.normal).normalized;
            var target = slopeDir * settings.maxSlopeSpeed;
            flags.slideVelocity = flags.slideVelocity.ExpDecay(target, settings.slopeAcceleration, Time.deltaTime);
            flags.horizontalVelocity += flags.slideVelocity;
        }

        void ApplyMove()
        {
            flags.finalMoveVector = flags.horizontalVelocity + Vector3.up * flags.verticalVelocity;
            controller.Move(flags.finalMoveVector * Time.deltaTime);
        }
    }
}