using System;
using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement.Data;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Look;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using System.Threading;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour, IUpdate
    {
        [Header("References")]
        [SerializeField] CameraController cameraController;
        [SerializeField] CharacterController characterController;
        [Header("Data")]
        [SerializeField] HeadBobData headBobData;
        [Header("Locomotion Settings")]
        [SerializeField] float crouchSpeed = 1f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 3f;
        [SerializeField] float slideSpeed = 7f;
        [SerializeField, Range(0f, 1f)] float moveBackwardsSpeedPercent = 0.5f;
        [SerializeField, Range(0f, 1f)] float moveSideSpeedPercent = 0.75f;
        [SerializeField] float displacementSpeed = 0.05f;
        [Header("Run Settings")]
        [SerializeField, Range(-1f, 1f)] float canRunThreshold = 0.8f;
        [SerializeField] AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Crouch Settings")]
        [SerializeField, Range(0.2f, 0.9f)] float crouchPercent = 0.6f;
        [SerializeField] float crouchTransitionDuration = 1f;
        [SerializeField] AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Landing Settings")]
        [SerializeField, Range(0.05f, 0.5f)] float lowLandAmount = 0.1f;
        [SerializeField, Range(0.2f, 0.9f)] float highLandAmount = 0.6f;
        [SerializeField] float landTimer = 0.5f;
        [SerializeField] float landDuration = 1f;
        [SerializeField] AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Gravity Settings")]
        [SerializeField] float gravityMultiplier = 2.5f;
        [SerializeField] float stickToGroundForce = 5f;
        [SerializeField] LayerMask groundLayer = ~0;
        [SerializeField, Range(0f, 1f)] float rayLength = 0.1f;
        [SerializeField, Range(0.01f, 1f)] float raySphereRadius = 0.1f;
        [Header("Check Wall Settings")]
        [SerializeField] LayerMask obstacleLayers = ~0;
        [SerializeField, Range(0f, 1f)] float rayObstacleLength = 0.1f;
        [SerializeField, Range(0.01f, 1f)] float rayObstacleSphereRadius = 0.1f;
        [Header("Smooth Settings")]
        [SerializeField] float smoothRotateSpeed = 5f;
        [SerializeField] float smoothInputSpeed = 5f;
        [SerializeField] float smoothVelocitySpeed = 5f;
        [SerializeField] float smoothFinalDirectionSpeed = 5f;
        [SerializeField] float smoothHeadBobSpeed = 5f;
        InputManager input;
        HeadBob headBob;
        CancellationTokenSource landingCancellationSource;
        CancellationTokenSource crouchCancellationSource;
        Transform yawTransform;
        RaycastHit hitInfo;
        Vector3 finalMoveDir;
        Vector3 smoothFinalMoveDir;
        Vector3 finalMoveVector;
        Vector3 initCenter;
        Vector3 crouchCenter;
        Vector2 smoothInputVector;
        float currentSpeed;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;
        float walkRunSpeedDifference;
        float finalRayLength;
        float initHeight;
        float initCamHeight;
        float crouchHeight;
        float inAirTimer;
        float crouchCamHeight;
        float jumpSpeed;
        bool duringCrouchAnimation;
        bool duringRunAnimation;
        bool hitWall;
        bool isCrouching;
        bool isRunning;
        bool isGrounded;
        bool previouslyGrounded;

        bool CanJump => !isCrouching && characterController.isGrounded;

        void OnEnable() => UpdateManager.RegisterUpdate(this);

        void Start()
        {
            GetComponents();
            InitVariables();
            SubscribeInputs();
        }

        void IUpdate.Update()
        {
            RotateTowardsCamera();
            ComputeCollisions();
            CheckIfGrounded();
            CheckIfWall();
            SmoothInput();
            SmoothSpeed();
            SmoothDir();
            CalculateMovementDirection();
            CalculateSpeed();
            CalculateFinalMovement();
            HandleHeadBob();
            HandleRunFOV();
            HandleCameraSway();
            HandleLanding();
            ApplyGravity();
            ApplyMovement();
            previouslyGrounded = isGrounded;
        }

        void OnDisable()
        {
            UnsubscribeInputs();
            UpdateManager.UnregisterUpdate(this);
        }

        void SubscribeInputs()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnSprintPressed += OnSprintPressed;
            input.OnSprintReleased += OnSprintReleased;
            input.OnCrouchPressed += OnCrouchPressed;
            input.OnJumpPressed += OnJumpPressed;
        }

        void UnsubscribeInputs()
        {
            input.OnSprintPressed -= OnSprintPressed;
            input.OnSprintReleased -= OnSprintReleased;
            input.OnCrouchPressed -= OnCrouchPressed;
            input.OnJumpPressed -= OnJumpPressed;
        }

        void OnSprintPressed()
        {
            isRunning = true;
            ChangeToRunFOV();
        }

        void OnSprintReleased()
        {
            isRunning = false;
            ChangeToInitFOV();
        }

        void OnCrouchPressed() => HandleCrouchInput();

        void OnJumpPressed() => HandleJump();

        void GetComponents()
        {
            characterController = GetComponent<CharacterController>();
            cameraController = GetComponentInChildren<CameraController>();
            yawTransform = cameraController.transform;
            headBob = new HeadBob(headBobData, moveBackwardsSpeedPercent, moveSideSpeedPercent);
        }

        void InitVariables()
        {
            characterController.center = new Vector3(
                0f, characterController.height / 2f + characterController.skinWidth, 0f);

            initCenter = characterController.center;
            initHeight = characterController.height;
            initCamHeight = yawTransform.localPosition.y;

            crouchHeight = initHeight * crouchPercent;
            crouchCenter = (crouchHeight / 2f + characterController.skinWidth) * Vector3.up;
            var crouchStandHeightDifference = initHeight - crouchHeight;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;

            finalRayLength = rayLength + characterController.center.y;

            isGrounded = true;
            previouslyGrounded = true;

            inAirTimer = 0f;
            headBob.CurrentStateHeight = initCamHeight;

            walkRunSpeedDifference = runSpeed - walkSpeed;

            jumpSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * 2f);
        }

        void SmoothInput() => smoothInputVector = smoothInputVector.ExpDecay(
            input.MovementAxis, smoothInputSpeed, Time.deltaTime);

        void SmoothSpeed()
        {
            smoothCurrentSpeed = smoothCurrentSpeed.ExpDecay(currentSpeed, smoothVelocitySpeed, Time.deltaTime);

            if (isRunning && CanRun())
            {
                var walkRunPercent = walkSpeed.InverseEerp(runSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(walkRunPercent) * walkRunSpeedDifference + walkSpeed;
                return;
            }

            finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }

        void SmoothDir() => smoothFinalMoveDir = smoothFinalMoveDir.ExpDecay(
            finalMoveDir, smoothFinalDirectionSpeed, Time.deltaTime);

        void CheckIfGrounded()
        {
            var origin = transform.position + characterController.center;
            isGrounded = Physics.SphereCast(
                origin, raySphereRadius, Vector3.down, out hitInfo,
                finalRayLength, groundLayer);

            //Debug.DrawRay(origin, Vector3.down * finalRayLength, isGrounded ? Color.red : Color.green);
        }

        void ComputeCollisions()
        {
            var colliding = characterController.GetPenetrationsInLayer(obstacleLayers, out var correction);
            correction += correction.normalized * 0.001f;
            if (colliding)
                transform.position += Vector3.zero.ExpDecay(correction, displacementSpeed, Time.deltaTime);
        }

        void CheckIfWall()
        {
            var origin = transform.position + characterController.center;
            if (input.MovementAxis != Vector2.zero && finalMoveDir.sqrMagnitude > 0f)
                hitWall = Physics.SphereCast(
                    origin, rayObstacleSphereRadius, finalMoveDir,
                    out var _, rayObstacleLength, obstacleLayers);

            //Debug.DrawRay(origin, finalMoveDir * rayObstacleLength, hitWall ? Color.red : Color.green);
        }

        bool CheckIfRoof() => Physics.SphereCast(transform.position, raySphereRadius, Vector3.up, out var _, initHeight);

        bool CanRun()
        {
            var normalizedDir = Vector3.zero;
            if (smoothFinalMoveDir != Vector3.zero)
                normalizedDir = smoothFinalMoveDir.normalized;

            var dot = Vector3.Dot(transform.forward, normalizedDir);
            return dot >= canRunThreshold && !isCrouching;
        }

        void CalculateMovementDirection()
        {
            var vDir = transform.forward * smoothInputVector.y;
            var hDir = transform.right * smoothInputVector.x;
            var desiredDir = vDir + hDir;
            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            finalMoveDir = flattenDir;
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
        {
            if (isGrounded) vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, hitInfo.normal);
            return vectorToFlat;
        }

        void CalculateSpeed()
        {
            currentSpeed = isRunning && CanRun() ? runSpeed : walkSpeed;
            currentSpeed = isCrouching ? crouchSpeed : currentSpeed;
            currentSpeed = input.MovementAxis == Vector2.zero ? 0f : currentSpeed;
            currentSpeed = input.MovementAxis.y == -1f ?
                currentSpeed * moveBackwardsSpeedPercent : currentSpeed;
            currentSpeed = input.MovementAxis.x != 0f && input.MovementAxis.y == 0f ?
                currentSpeed * moveSideSpeedPercent : currentSpeed;
        }

        void CalculateFinalMovement()
        {
            var finalVector = smoothFinalMoveDir * finalSmoothCurrentSpeed;
            finalMoveVector.x = finalVector.x;
            finalMoveVector.z = finalVector.z;
            if (characterController.isGrounded)
                finalMoveVector.y += finalVector.y;
        }

        void HandleCrouchInput()
        {
            if (isGrounded)
                _ = InvokeCrouchRoutine();
        }

        async Awaitable InvokeCrouchRoutine()
        {
            if (CheckIfRoof()) return;

            if (landingCancellationSource != null)
            {
                landingCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }

            if (crouchCancellationSource != null)
            {
                crouchCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }

            crouchCancellationSource = new CancellationTokenSource();
            await HandleCrouch(crouchCancellationSource.Token);
        }

        async Awaitable HandleCrouch(CancellationToken cancellationToken)
        {
            duringCrouchAnimation = true;

            var percent = 0f;
            var speed = 1f / crouchTransitionDuration;

            var currentHeight = characterController.height;
            var currentCenter = characterController.center;

            var desiredHeight = isCrouching ? initHeight : crouchHeight;
            var desiredCenter = isCrouching ? initCenter : crouchCenter;

            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

            isCrouching = !isCrouching;
            headBob.CurrentStateHeight = isCrouching ? crouchCamHeight : initCamHeight;

            while (percent < 1f)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                percent += Time.deltaTime * speed;
                var smoothPercent = crouchTransitionCurve.Evaluate(percent);

                characterController.height = Mathf.Lerp(currentHeight, desiredHeight, smoothPercent);
                characterController.center = Vector3.Lerp(currentCenter, desiredCenter, smoothPercent);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, smoothPercent);
                yawTransform.localPosition = camPos;

                await Awaitable.NextFrameAsync();
            }

            duringCrouchAnimation = false;
        }

        void HandleLanding()
        {
            if (!previouslyGrounded && isGrounded)
                _ = InvokeLandingRoutine();
        }

        async Awaitable InvokeLandingRoutine()
        {
            if (landingCancellationSource != null)
            {
                landingCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }

            landingCancellationSource = new CancellationTokenSource();
            await LandingRoutine(landingCancellationSource.Token);
        }

        async Awaitable LandingRoutine(CancellationToken cancellationToken)
        {
            var percent = 0f;
            var speed = 1f / landDuration;

            var localPos = yawTransform.localPosition;
            var initLandHeight = localPos.y;

            var landAmount = inAirTimer > landTimer ? highLandAmount : lowLandAmount;

            while (percent < 1f)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                percent += Time.deltaTime * speed;
                var desiredY = landCurve.Evaluate(percent) * landAmount;

                localPos.y = initLandHeight + desiredY;
                yawTransform.localPosition = localPos;

                await Awaitable.NextFrameAsync();
            }
        }

        void HandleHeadBob()
        {
            if (input.MovementAxis != Vector2.zero && isGrounded && !hitWall)
            {
                if (!duringCrouchAnimation)
                {
                    headBob.ScrollHeadBob(isRunning && CanRun(), isCrouching, input.MovementAxis);

                    yawTransform.localPosition = yawTransform.localPosition.ExpDecay(
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        smoothHeadBobSpeed, Time.deltaTime);
                }
            }
            else
            {
                if (!headBob.Resetted)
                    headBob.ResetHeadBob();

                if (!duringCrouchAnimation)
                    yawTransform.localPosition = yawTransform.localPosition.ExpDecay(
                        new Vector3(0f, headBob.CurrentStateHeight, 0f),
                        smoothHeadBobSpeed, Time.deltaTime);
            }
        }

        void HandleCameraSway() => cameraController.HandleSway(smoothInputVector, input.MovementAxis.x);

        void HandleRunFOV()
        {
            if (!duringRunAnimation && input.MovementAxis != Vector2.zero && !hitWall && isRunning && CanRun())
            {
                duringRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }

            if (duringRunAnimation && (input.MovementAxis == Vector2.zero || !CanRun() || hitWall))
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }

        void ChangeToRunFOV()
        {
            if (!CanRun() || input.MovementAxis == Vector2.zero) return;
            duringRunAnimation = true;
            cameraController.ChangeRunFOV(false);
        }

        void ChangeToInitFOV()
        {
            if (!duringRunAnimation) return;
            duringRunAnimation = false;
            cameraController.ChangeRunFOV(true);
        }

        void HandleJump()
        {
            if (!CanJump) return;
            finalMoveVector.y = jumpSpeed;
            previouslyGrounded = true;
            isGrounded = false;
        }

        void ApplyGravity()
        {
            if (characterController.isGrounded)
            {
                inAirTimer = 0f;
                finalMoveVector.y = Mathf.Clamp(
                    finalMoveVector.y -= stickToGroundForce * Time.deltaTime, -stickToGroundForce, jumpSpeed);
            }
            else
            {
                inAirTimer += Time.deltaTime;
                finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
            }
        }

        void ApplyMovement()
        {
            var flags = characterController.Move(finalMoveVector * Time.deltaTime);
            if ((flags & CollisionFlags.Above) != 0f)
                finalMoveVector.y = -0.5f;
        }

        void RotateTowardsCamera() => transform.rotation = transform.rotation.ExpDecay(
            yawTransform.rotation, smoothRotateSpeed, Time.deltaTime);
    }
}