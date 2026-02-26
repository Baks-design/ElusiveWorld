using UnityEngine;
using KinematicCharacterController;
using System.Collections.Generic;

namespace ElusiveWorld.Core.Character
{
    public class PlayerCharacter : MonoBehaviour, ICharacterController
    {
        [Header("References")]
        [SerializeField] KinematicCharacterMotor motor;
        [SerializeField] PlayerCamera playerCamera;
        [SerializeField] Transform root;
        [SerializeField] Transform cameraTarget;
        [SerializeField] List<Collider> ignoredColliders = new();
        [Header("HeadBob Settings")]
        [SerializeField] HeadBobData headBobData;
        [SerializeField] float smoothHeadBobSpeed = 10f;
        [SerializeField, Range(0f, 1f)] float moveBackwardsSpeedPercent = 0.5f;
        [SerializeField, Range(0f, 1f)] float moveSideSpeedPercent = 0.75f;
        [Header("Walk Settings")]
        [SerializeField] float walkSpeed = 5f;
        [SerializeField] float runSpeed = 15f;
        [SerializeField] float walkResponse = 10f;
        [SerializeField] float runResponse = 25f;
        [Space, Header("Run Settings")]
        [SerializeField, Range(-1f, 1f)] float canRunThreshold = 0.8f;
        [Header("Air Settings")]
        [SerializeField] float airSpeed = 15f;
        [SerializeField] float airAcceleration = 70f;
        [Header("Jump Settings")]
        [SerializeField] float jumpSpeed = 20f;
        [SerializeField] float coyoteTime = 0.2f;
        [SerializeField, Range(0f, 1f)] float jumpSustainGravity = 0.4f;
        [SerializeField] float gravity = -90f;
        [Header("Slide Settings")]
        [SerializeField] float slideStartSpeed = 25f;
        [SerializeField] float slideEndSpeed = 15f;
        [SerializeField] float slideFriction = 0.8f;
        [SerializeField] float slideSteerAcceleration = 5f;
        [SerializeField] float slideGravity = 90f;
        [Header("Crouch Settings")]
        [SerializeField] float crouchSpeed = 7f;
        [SerializeField] float crouchResponse = 20f;
        [SerializeField] float standHeight = 2f;
        [SerializeField] float crouchHeight = 1f;
        [SerializeField] float crouchHeightResponse = 15f;
        [SerializeField, Range(0f, 1f)] float standCameraTargetHeight = 0.9f;
        [SerializeField, Range(0f, 1f)] float crouchCameraTargetHeight = 0.7f;
        readonly Stance stance;
        CharacterState state;
        CharacterState lastState;
        CharacterState tempState;
        Collider[] uncrouchOverlapResults;
        Quaternion requestedRotation;
        Vector3 requestedMovement;
        Vector3 internalVelocityAdd;
        Transform yawTransform;
        HeadBob headBob;
        bool requestedSprint;
        bool requestedJump;
        bool requestedSustainedJump;
        bool requestedCrouch;
        bool requestedCrouchInAir;
        bool ungroundedDueToJump;
        bool duringRunAnimation;
        bool duringCrouchAnimation;
        float timeSinceUngrounded;
        float timeSinceJumpRequest;
        float initCamHeight;
        float crouchCamHeight;

        public Transform GetCameraTarget => cameraTarget;
        public CharacterState GetState => state;
        public CharacterState GetLastState => lastState;

        public void Initialize()
        {
            motor.CharacterController = this;

            state.Stance = Stance.Stand;
            lastState = state;

            internalVelocityAdd = Vector3.zero;
            uncrouchOverlapResults = new Collider[8];

            var crouchStandHeightDifference = motor.Capsule.height - crouchHeight;
            yawTransform = playerCamera.transform;
            initCamHeight = yawTransform.localPosition.y;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;
            duringCrouchAnimation = false;

            headBob = new HeadBob(headBobData, moveBackwardsSpeedPercent, moveSideSpeedPercent)
            {
                CurrentStateHeight = initCamHeight
            };
        }

        public void SetPosition(Vector3 position, bool killVelocity = true)
        {
            motor.SetPosition(position);
            if (killVelocity)
                motor.BaseVelocity = Vector3.zero;
        }

        public void AddVelocity(Vector3 velocity) => internalVelocityAdd += velocity;

        public void UpdateInput(CharacterInput input) //TODO: Check Inputs
        {
            requestedRotation = input.Rotation;
            requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
            requestedMovement = Vector3.ClampMagnitude(requestedMovement, 1f);
            requestedMovement = input.Rotation * requestedMovement;

            requestedSprint = input.Sprint;

            var wasRequestingJump = requestedJump;
            requestedJump = requestedJump || input.Jump;
            if (requestedJump && !wasRequestingJump)
                timeSinceJumpRequest = 0f;
            requestedSustainedJump = input.JumpSustain;

            var wasRequestingCrouch = requestedCrouch;
            requestedCrouch = input.Crouch switch
            {
                CrouchInput.Toggle => !requestedCrouch,
                CrouchInput.None => requestedCrouch,
                _ => requestedCrouch
            };
            if (requestedCrouch && !wasRequestingCrouch)
                requestedCrouchInAir = !state.Grounded;
            else if (!requestedCrouch && wasRequestingCrouch)
                requestedCrouchInAir = false;
        }

        public void UpdateBody(float deltaTime)
        {
            var currentHeight = motor.Capsule.height;
            var normalizeHeight = currentHeight / standHeight;

            var cameraTargetHeight = currentHeight *
            (
                stance is Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight
            );
            var rootTargetScale = new Vector3(1f, normalizeHeight, 1f);

            cameraTarget.localPosition = Vector3.Lerp(
                cameraTarget.localPosition,
                new Vector3(0f, cameraTargetHeight, 0f),
                1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
            );
            root.localScale = Vector3.Lerp(
                root.localScale,
                rootTargetScale,
                1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
            );
        }

        #region Velocity
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            state.Acceleration = Vector3.zero;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                timeSinceUngrounded = 0f;
                ungroundedDueToJump = false;

                var groundedMovement = motor.GetDirectionTangentToSurface
                (
                    requestedMovement,
                    motor.GroundingStatus.GroundNormal
                ) * requestedMovement.magnitude;

                StartSliding(ref currentVelocity, groundedMovement);
                Move(ref currentVelocity, deltaTime, groundedMovement);
            }
            // in the air
            else
            {
                timeSinceUngrounded += deltaTime;

                if (requestedMovement.sqrMagnitude > 0f)
                {
                    var projection = Vector3.ProjectOnPlane(requestedMovement, motor.CharacterUp).normalized;
                    var planarMovement = projection * requestedMovement.magnitude;
                    var movementForce = airAcceleration * deltaTime * planarMovement;

                    var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                    if (currentPlanarVelocity.magnitude < airSpeed)
                    {
                        var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                        targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                        movementForce = targetPlanarVelocity - currentPlanarVelocity;
                    }
                    else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                    {
                        var constraineMovementForce = Vector3.ProjectOnPlane(movementForce, currentPlanarVelocity.normalized);
                        movementForce = constraineMovementForce;
                    }

                    if (motor.GroundingStatus.FoundAnyGround)
                    {
                        if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0)
                        {
                            var obstructionNormal = Vector3.Cross
                            (
                                motor.CharacterUp, Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)
                            ).normalized;
                            movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                        }
                    }

                    currentVelocity += movementForce;
                }

                Gravity(ref currentVelocity, deltaTime);
            }

            if (requestedJump)
            {
                var grounded = motor.GroundingStatus.IsStableOnGround;
                var canCoyoteJump = timeSinceUngrounded < coyoteTime && !ungroundedDueToJump;

                if (grounded || canCoyoteJump)
                {
                    requestedJump = false;
                    requestedCrouch = false;
                    requestedCrouchInAir = false;

                    motor.ForceUnground(0.1f);
                    ungroundedDueToJump = true;

                    var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                    var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                    currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
                }
                else
                {
                    timeSinceJumpRequest += deltaTime;

                    var canJumpLater = timeSinceJumpRequest < coyoteTime;
                    requestedJump = canJumpLater;
                }
            }

            if (internalVelocityAdd.sqrMagnitude > 0f)
            {
                currentVelocity += internalVelocityAdd;
                internalVelocityAdd = Vector3.zero;
            }

            UpdateFOV(ref currentVelocity);
            UpdateHeadBob(ref currentVelocity, deltaTime);
        }

        void StartSliding(ref Vector3 currentVelocity, Vector3 groundedMovement)
        {
            var moving = groundedMovement.sqrMagnitude > 0f;
            var crouching = state.Stance is Stance.Crouch;
            var wasStanding = lastState.Stance is Stance.Stand;
            var wasInAir = !lastState.Grounded;
            if (moving && crouching && (wasStanding || wasInAir))
            {
                state.Stance = Stance.Slide;

                if (wasInAir)
                    currentVelocity = Vector3.ProjectOnPlane(lastState.Velocity, motor.GroundingStatus.GroundNormal);

                var effecttiveSlideStarSpeed = slideStartSpeed;
                if (!lastState.Grounded && !requestedCrouchInAir)
                {
                    effecttiveSlideStarSpeed = 0f;
                    requestedCrouchInAir = false;
                }

                var slideSpeed = Mathf.Max(effecttiveSlideStarSpeed, currentVelocity.magnitude);
                currentVelocity = motor.GetDirectionTangentToSurface(
                    currentVelocity, motor.GroundingStatus.GroundNormal
                ) * slideSpeed;
            }
        }

        void Move(ref Vector3 currentVelocity, float deltaTime, Vector3 groundedMovement)
        {
            if (state.Stance is Stance.Stand or Stance.Crouch)
            {
                var speed = CalculateSpeed(ref currentVelocity);
                var response = CalculateResponse(ref currentVelocity);

                var targetVelocity = groundedMovement * speed;
                var moveVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-response * deltaTime));

                state.Acceleration = moveVelocity - currentVelocity;

                currentVelocity = moveVelocity;
            }
            // Continue sliding
            else
            {
                // Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                Slope(ref currentVelocity, deltaTime);
                Steer(ref currentVelocity, deltaTime, groundedMovement);

                // Stop
                if (currentVelocity.magnitude < slideEndSpeed)
                    state.Stance = Stance.Crouch;
            }
        }

        float CalculateSpeed(ref Vector3 currentVelocity) //TODO: Check Speeds
        {
            float currentSpeed;
            currentSpeed = requestedSprint && CanRun(ref currentVelocity) ? runSpeed : walkSpeed;
            currentSpeed = stance is Stance.Stand ? crouchSpeed : currentSpeed;
            currentSpeed = requestedMovement == Vector3.zero ? 0f : currentSpeed;
            currentSpeed = requestedMovement.z == -1f ? currentSpeed * moveBackwardsSpeedPercent : currentSpeed;
            currentSpeed = requestedMovement.x != 0f && requestedMovement.z == 0f ?
                currentSpeed * moveSideSpeedPercent : currentSpeed;
            return currentSpeed;
        }

        float CalculateResponse(ref Vector3 currentVelocity)
        {
            float response;
            response = requestedSprint && CanRun(ref currentVelocity) ? runResponse : walkResponse;
            response = stance is Stance.Stand ? response : crouchResponse;
            return response;
        }

        void Slope(ref Vector3 currentVelocity, float deltaTime)
        {
            var force = Vector3.ProjectOnPlane(-motor.CharacterUp, motor.GroundingStatus.GroundNormal) * slideGravity;
            currentVelocity -= force * deltaTime;
        }

        void Steer(ref Vector3 currentVelocity, float deltaTime, Vector3 groundedMovement)
        {
            var currentSpeed = currentVelocity.magnitude;
            var targertVelocity = groundedMovement * currentSpeed;
            var steerVelocity = currentVelocity;
            var steerForce = deltaTime * slideSteerAcceleration * (targertVelocity - steerVelocity);
            steerVelocity += steerForce;
            steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

            state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;

            currentVelocity = steerVelocity;
        }

        void Gravity(ref Vector3 currentVelocity, float deltaTime)
        {
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (requestedSustainedJump && verticalSpeed > 0f)
                effectiveGravity *= jumpSustainGravity;
            currentVelocity += deltaTime * effectiveGravity * motor.CharacterUp;
        }

        void UpdateFOV(ref Vector3 currentVelocity)
        {
            if (requestedSprint &
                requestedMovement != Vector3.zero &&
                motor.GroundingStatus.IsStableOnGround &&
                CanRun(ref currentVelocity))
            {
                duringRunAnimation = true;
                playerCamera.UpdateRunFOV(false);
            }

            if ((!requestedSprint || requestedMovement == Vector3.zero) && duringRunAnimation)
            {
                duringRunAnimation = false;
                playerCamera.UpdateRunFOV(true);
            }
        }

        bool CanRun(ref Vector3 currentVelocity)
        {
            var normalizedDir = Vector3.zero;
            if (currentVelocity != Vector3.zero)
                normalizedDir = currentVelocity.normalized;

            var dot = Vector3.Dot(transform.forward, normalizedDir);
            return dot >= canRunThreshold && stance is Stance.Stand;
        }

        void UpdateHeadBob(ref Vector3 currentVelocity, float deltaTime)
        {
            headBob.CurrentStateHeight = state.Stance is not Stance.Stand ? crouchCamHeight : initCamHeight;

            if (requestedMovement != Vector3.zero && motor.GroundingStatus.IsStableOnGround)
            {
                if (!duringCrouchAnimation)
                {
                    headBob.ScrollHeadBob(
                        requestedSprint && CanRun(ref currentVelocity),
                        requestedCrouch,
                        new Vector2(requestedMovement.x, requestedMovement.z));

                    yawTransform.localPosition = Vector3.Lerp(
                        yawTransform.localPosition,
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        1f - Mathf.Exp(-smoothHeadBobSpeed * deltaTime));
                }
            }
            else
            {
                if (!headBob.Resetted)
                    headBob.ResetHeadBob();

                if (!duringCrouchAnimation)
                    yawTransform.localPosition = Vector3.Lerp(
                        yawTransform.localPosition,
                        new Vector3(0f, headBob.CurrentStateHeight, 0f),
                        1f - Mathf.Exp(-smoothHeadBobSpeed * deltaTime)
                    );
            }
        }
        #endregion

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var forward = Vector3.ProjectOnPlane(requestedRotation * Vector3.forward, motor.CharacterUp);
            if (forward != Vector3.zero)
                currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            tempState = state;

            if (requestedCrouch && state.Stance is Stance.Stand)
            {
                state.Stance = Stance.Crouch;
                motor.SetCapsuleDimensions(motor.Capsule.radius, crouchHeight, crouchHeight * 0.5f);
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (!motor.GroundingStatus.IsStableOnGround && state.Stance is Stance.Slide)
                state.Stance = Stance.Crouch;

            if (motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
                OnLanded();
            else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround)
                OnLeaveStableGround();
        }

        void OnLanded() { }

        void OnLeaveStableGround() { }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (!requestedCrouch && state.Stance is not Stance.Stand)
            {
                motor.SetCapsuleDimensions(motor.Capsule.radius, standHeight, standHeight * 0.5f);

                var pos = motor.TransientPosition;
                var rot = motor.TransientRotation;
                var mask = motor.CollidableLayers;
                if (motor.CharacterOverlap(pos, rot, uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0)
                {
                    requestedCrouch = true;
                    motor.SetCapsuleDimensions(motor.Capsule.radius, crouchHeight, crouchHeight * 0.5f);
                }
                else
                    state.Stance = Stance.Stand;
            }

            var totalAcceleration = (state.Velocity - lastState.Velocity) / deltaTime;
            state.Acceleration = Vector3.ClampMagnitude(state.Acceleration, totalAcceleration.magnitude);

            state.Grounded = motor.GroundingStatus.IsStableOnGround;
            state.Velocity = motor.Velocity;
            lastState = tempState;
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (ignoredColliders.Count == 0) return true;
            if (ignoredColliders.Contains(coll)) return false;
            return true;
        }

        public void OnGroundHit(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        { }

        public void OnMovementHit(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) =>
            state.Acceleration = Vector3.ProjectOnPlane(state.Acceleration, hitNormal);

        public void ProcessHitStabilityReport(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        { }

        public void OnDiscreteCollisionDetected(Collider hitCollider) { }
    }
}