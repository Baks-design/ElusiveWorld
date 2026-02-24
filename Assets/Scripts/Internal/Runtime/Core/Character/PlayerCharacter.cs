using UnityEngine;
using KinematicCharacterController;

namespace ElusiveWorld.Core.Character
{
    public enum CrouchInput { None, Toggle }
    public enum Stance { Stand, Crouch, Slide }

    public struct CharacterState
    {
        public bool Grounded;
        public Stance Stance;
    }
    public struct CharacterInput
    {
        public Quaternion Rotation;
        public Vector2 Move;
        public bool Jump;
        public bool JumpSustain;
        public CrouchInput Crouch;
    }

    public class PlayerCharacter : MonoBehaviour, ICharacterController
    {
        [SerializeField] KinematicCharacterMotor motor;
        [SerializeField] Transform root;
        [SerializeField] Transform cameraTarget;
        [Space]
        [SerializeField] float walkSpeed = 20f;
        [SerializeField] float crouchSpeed = 7f;
        [SerializeField] float walkResponse = 25f;
        [SerializeField] float crouchResponse = 20f;
        [Space]
        [SerializeField] float airSpeed = 15f;
        [SerializeField] float airAcceleration = 70f;
        [Space]
        [SerializeField] float jumpSpeed = 20f;
        [SerializeField, Range(0f, 1f)] float jumpSustainGravity = 0.4f;
        [SerializeField] float gravity = -90f;
        [Space]
        [SerializeField] float slideStartSpeed = 25f;
        [SerializeField] float slideEndSpeed = 15f;
        [SerializeField] float slideFriction = 0.8f;
        [Space]
        [SerializeField] float standHeight = 2f;
        [SerializeField] float crouchHeight = 1f;
        [SerializeField] float crouchHeightResponse = 15f;
        [SerializeField, Range(0f, 1f)] float standCameraTargetHeight = 0.9f;
        [SerializeField, Range(0f, 1f)] float crouchCameraTargetHeight = 0.7f;
        readonly Stance stance;
        CharacterState state;
        CharacterState lastState;
        CharacterState tempState;
        Quaternion requestedRotation;
        Vector3 requestedMovement;
        bool requestedJump;
        bool requestedSustainedJump;
        bool requestedCrouch;
        Collider[] uncrouchOverlapResults;

        public void Initialize()
        {
            state.Stance = Stance.Stand;
            lastState = state;

            uncrouchOverlapResults = new Collider[8];

            motor.CharacterController = this;
        }

        public void UpdateInput(CharacterInput input)
        {
            requestedRotation = input.Rotation;
            requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
            requestedMovement = Vector3.ClampMagnitude(requestedMovement, 1f);
            requestedMovement = input.Rotation * requestedMovement;

            requestedJump = requestedJump || input.Jump;
            requestedSustainedJump = input.JumpSustain;

            requestedCrouch = input.Crouch switch
            {
                CrouchInput.Toggle => !requestedCrouch,
                CrouchInput.None => requestedCrouch,
                _ => requestedCrouch
            };
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

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (motor.GroundingStatus.IsStableOnGround)
            {
                var groundedMovement = motor.GetDirectionTangentToSurface
                (
                    requestedMovement,
                    motor.GroundingStatus.GroundNormal
                ) * requestedMovement.magnitude;

                //Start Sliding
                {
                    var moving = groundedMovement.sqrMagnitude > 0f;
                    var crouching = state.Stance is Stance.Crouch;
                    var wasStanding = lastState.Stance is Stance.Stand;
                    var wasInAir = !lastState.Grounded;
                    if (moving && crouching && (wasStanding || wasInAir))
                    {
                        state.Stance = Stance.Slide;

                        var slideSpeed = Mathf.Max(slideStartSpeed, currentVelocity.magnitude);
                        currentVelocity = motor.GetDirectionTangentToSurface
                        (
                            currentVelocity,
                            motor.GroundingStatus.GroundNormal
                        ) * slideSpeed;
                    }
                }
                //Move
                {
                    if (state.Stance is Stance.Stand or Stance.Crouch)
                    {
                        var speed = stance is Stance.Stand ? walkSpeed : crouchSpeed;
                        var response = stance is Stance.Stand ? walkResponse : crouchResponse;

                        var targetVelocity = groundedMovement * speed;
                        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-response * deltaTime));
                    }
                    else
                    {
                        currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                        if (currentVelocity.magnitude < slideEndSpeed)
                            state.Stance = Stance.Crouch;
                    }
                }
            }
            else
            {
                if (requestedMovement.sqrMagnitude > 0f)
                {
                    var planarMovement = Vector3.ProjectOnPlane(
                        requestedMovement, motor.CharacterUp).normalized * requestedMovement.magnitude;
                    var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                    var movementForce = airAcceleration * deltaTime * planarMovement;
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                    currentVelocity += targetPlanarVelocity - currentPlanarVelocity;
                }

                var effectiveGravity = gravity;
                var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                if (requestedSustainedJump && verticalSpeed > 0f)
                    effectiveGravity *= jumpSustainGravity;
                currentVelocity += deltaTime * effectiveGravity * motor.CharacterUp;
            }

            if (requestedJump)
            {
                requestedJump = false;
                requestedCrouch = false;

                motor.ForceUnground(0.1f);
                
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
        }

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
        }

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

            state.Grounded = motor.GroundingStatus.IsStableOnGround;
            lastState = tempState;
        }

        public bool IsColliderValidForCollisions(Collider coll) => true;
        public void OnGroundHit(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        { }
        public void OnMovementHit(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        { }
        public void ProcessHitStabilityReport(
            Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        { }
        public void OnDiscreteCollisionDetected(Collider hitCollider) { }

        public Transform GetCameraTarget() => cameraTarget;

        public void SetPosition(Vector3 position, bool killVelocity = true)
        {
            motor.SetPosition(position);
            if (killVelocity)
                motor.BaseVelocity = Vector3.zero;
        }
    }
}