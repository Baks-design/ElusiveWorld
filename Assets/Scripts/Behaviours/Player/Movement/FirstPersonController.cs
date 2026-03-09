using System;
using UnityEngine;
using LitMotion;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement.Data;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Look;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
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
        [Header("Run Settings")]
        [SerializeField, Range(-1f, 1f)] float canRunThreshold = 0.8f;
        [SerializeField] AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Crouch Settings")]
        [SerializeField, Range(0.2f, 0.9f)] float crouchPercent = 0.6f;
        [SerializeField] float crouchTransitionDuration = 1f;
        [SerializeField] AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Header("Slide Settings")]
        [SerializeField, Range(0.2f, 0.9f)] float slidePercent = 0.6f;
        [SerializeField] float slideTransitionDuration = 1f;
        [SerializeField] float maxSlideDuration = 2f;
        [SerializeField] AnimationCurve slideTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
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
        CompositeMotionHandle slideMotionHandles;
        CompositeMotionHandle returnMotionHandles;
        CompositeMotionHandle crouchMotionHandles;
        MotionHandle landMotionHandle;
        Transform yawTransform;
        RaycastHit hitInfo;
        Vector3 finalMoveDir;
        Vector3 smoothFinalMoveDir;
        Vector3 finalMoveVector;
        Vector3 initCenter;
        Vector3 crouchCenter;
        Vector3 slideCenter;
        Vector2 smoothInputVector;
        float currentSpeed;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;
        float walkRunSpeedDifference;
        float slideCamHeight;
        float finalRayLength;
        float initHeight;
        float initCamHeight;
        float crouchHeight;
        float inAirTimer;
        float crouchCamHeight;
        float slideHeight;
        float jumpSpeed;
        bool duringCrouchAnimation;
        bool duringRunAnimation;
        bool hitWall;
        bool isCrouching;
        bool isSliding;
        bool isRunning;
        bool isGrounded;
        bool previouslyGrounded;

        bool CanJump => !isSliding && !isCrouching && characterController.isGrounded;
        bool CanCrouch => characterController.isGrounded;

        void OnEnable()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnSprintPressed += OnSprintPressed;
            input.OnSprintReleased += OnSprintReleased;
            input.OnCrouchPressed += OnCrouchPressed;
            input.OnCrouchReleased += OnCrouchReleased;
            input.OnJumpPressed += OnJumpPressed;
        }

        void Start()
        {
            GetComponents();
            InitVariables();
        }

        void Update()
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
            previouslyGrounded = isGrounded;
        }

        void FixedUpdate()
        {
            ApplyGravity();
            ApplyMovement();
        }

        void OnDisable()
        {
            input.OnSprintPressed -= OnSprintPressed;
            input.OnSprintReleased -= OnSprintReleased;
            input.OnCrouchPressed -= OnCrouchPressed;
            input.OnCrouchReleased -= OnCrouchReleased;
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

        void OnCrouchReleased() => ReturnToInitHeight();

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

            slideHeight = initHeight * slidePercent;
            slideCenter = (slideHeight / 2f + characterController.skinWidth) * Vector3.up;
            var slideStandHeightDifference = initHeight - slideHeight;
            slideCamHeight = initCamHeight - slideStandHeightDifference;

            finalRayLength = rayLength + characterController.center.y;

            isGrounded = true;
            previouslyGrounded = true;

            inAirTimer = 0f;
            headBob.CurrentStateHeight = initCamHeight;

            walkRunSpeedDifference = runSpeed - walkSpeed;

            jumpSpeed = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * 2f);
        }

        void SmoothInput() => smoothInputVector = Vector2Extensions.ExpDecay(
            smoothInputVector, input.MovementAxis, smoothInputSpeed, Time.deltaTime);

        void SmoothSpeed()
        {
            smoothCurrentSpeed = FloatExtensions.ExpDecay(
                smoothCurrentSpeed, currentSpeed, smoothVelocitySpeed, Time.deltaTime);

            if (isRunning && CanRun() && !isSliding)
            {
                var walkRunPercent = FloatExtensions.InverseEerp(walkSpeed, runSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(walkRunPercent) * walkRunSpeedDifference + walkSpeed;
                return;
            }

            finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }

        void SmoothDir() => smoothFinalMoveDir = Vector3Extensions.ExpDecay(
            smoothFinalMoveDir, finalMoveDir, smoothFinalDirectionSpeed, Time.deltaTime);

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
                transform.position += Vector3Extensions.ExpDecay(Vector3.zero, correction, 0.05f, Time.deltaTime);
        }

        void CheckIfWall()
        {
            var origin = transform.position + characterController.center;
            if (input.MovementAxis != Vector2.zero && finalMoveDir.sqrMagnitude > 0)
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
            currentSpeed = isSliding ? slideSpeed : currentSpeed;
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
            if (!CanCrouch) return;

            if (isRunning && !isCrouching && input.MovementAxis != Vector2.zero && CanRun())
                HandleSlide();
            else
                HandleCrouch();
        }

        void HandleSlide()
        {
            isSliding = true;
            headBob.CurrentStateHeight = slideCamHeight;

            // Cancel any existing slide motions
            slideMotionHandles?.Cancel();
            slideMotionHandles = new CompositeMotionHandle();

            // Character controller height tween
            LMotion.Create(characterController.height, slideHeight, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(x => characterController.height = x)
                .AddTo(slideMotionHandles);

            // Character controller center tween
            LMotion.Create(characterController.center, slideCenter, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(x => characterController.center = x)
                .AddTo(slideMotionHandles);

            // Yaw transform local position Y tween
            LMotion.Create(yawTransform.localPosition.y, slideCamHeight, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(y =>
                {
                    var pos = yawTransform.localPosition;
                    pos.y = y;
                    yawTransform.localPosition = pos;
                })
                .AddTo(slideMotionHandles);

            this.Delay(maxSlideDuration, ReturnToInitHeight);
        }

        void ReturnToInitHeight()
        {
            if (CheckIfRoof())
            {
                slideMotionHandles?.Cancel();
                returnMotionHandles?.Cancel();
                crouchMotionHandles?.Cancel();

                isSliding = false;
                HandleCrouch();
                return;
            }

            if (!isSliding) return;

            slideMotionHandles?.Cancel();
            returnMotionHandles?.Cancel();
            returnMotionHandles = new CompositeMotionHandle();

            isSliding = false;
            headBob.CurrentStateHeight = initCamHeight;

            // Character controller height tween
            LMotion.Create(characterController.height, initHeight, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(x => characterController.height = x)
                .AddTo(returnMotionHandles);

            // Character controller center tween
            LMotion.Create(characterController.center, initCenter, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(x => characterController.center = x)
                .AddTo(returnMotionHandles);

            // Yaw transform local position Y tween
            LMotion.Create(yawTransform.localPosition.y, initCamHeight, slideTransitionDuration)
                .WithEase(slideTransitionCurve)
                .Bind(y =>
                {
                    var pos = yawTransform.localPosition;
                    pos.y = y;
                    yawTransform.localPosition = pos;
                })
                .AddTo(returnMotionHandles);
        }

        void HandleCrouch()
        {
            if (isCrouching)
                if (CheckIfRoof())
                    return;

            if (landMotionHandle.IsActive())
                landMotionHandle.Cancel();

            crouchMotionHandles?.Cancel();
            crouchMotionHandles = new CompositeMotionHandle();

            duringCrouchAnimation = true;

            var desiredHeight = isCrouching ? initHeight : crouchHeight;
            var desiredCenter = isCrouching ? initCenter : crouchCenter;
            var camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

            isCrouching = !isCrouching;
            headBob.CurrentStateHeight = isCrouching ? crouchCamHeight : initCamHeight;

            // Character controller height tween
            LMotion.Create(characterController.height, desiredHeight, crouchTransitionDuration)
                .WithEase(crouchTransitionCurve)
                .Bind(x => characterController.height = x)
                .AddTo(crouchMotionHandles);

            // Character controller center tween
            LMotion.Create(characterController.center, desiredCenter, crouchTransitionDuration)
                .WithEase(crouchTransitionCurve)
                .Bind(x => characterController.center = x)
                .AddTo(crouchMotionHandles);

            // Yaw transform local position Y tween with completion callback
            LMotion.Create(yawTransform.localPosition.y, camDesiredHeight, crouchTransitionDuration)
                .WithEase(crouchTransitionCurve)
                .WithOnComplete(() => duringCrouchAnimation = false)
                .Bind(y =>
                {
                    var pos = yawTransform.localPosition;
                    pos.y = y;
                    yawTransform.localPosition = pos;
                })
                .AddTo(crouchMotionHandles);
        }

        void HandleLanding()
        {
            if (!previouslyGrounded && isGrounded)
                InvokeLandingRoutine();
        }

        void InvokeLandingRoutine()
        {
            if (landMotionHandle.IsActive())
                landMotionHandle.Cancel();

            StartLandingMotion();
        }

        void StartLandingMotion()
        {
            var startPos = yawTransform.localPosition;
            var startHeight = startPos.y;
            var landAmount = inAirTimer > landTimer ? highLandAmount : lowLandAmount;

            // Create and bind the motion
            landMotionHandle = LMotion.Create(0f, 1f, landDuration)
                .Bind(x =>
                {
                    var pos = yawTransform.localPosition;
                    pos.y = startHeight + (landCurve.Evaluate(x) * landAmount);
                    yawTransform.localPosition = pos;
                });
        }

        void HandleHeadBob()
        {
            if (input.MovementAxis != Vector2.zero && isGrounded && !hitWall)
            {
                if (!duringCrouchAnimation && !isSliding)
                {
                    headBob.ScrollHeadBob(isRunning && CanRun(), isCrouching, input.MovementAxis);

                    yawTransform.localPosition = Vector3Extensions.ExpDecay(
                        yawTransform.localPosition,
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        smoothHeadBobSpeed, Time.deltaTime);
                }
            }
            else
            {
                if (!headBob.Resetted)
                    headBob.ResetHeadBob();

                if (!duringCrouchAnimation)
                    yawTransform.localPosition = Vector3Extensions.ExpDecay(
                        yawTransform.localPosition,
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
            if (!CanRun() || input.MovementAxis == Vector2.zero)
                return;

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

        void ApplyMovement() => characterController.Move(finalMoveVector * Time.deltaTime);

        void RotateTowardsCamera() => transform.rotation = QuaternionExtensions.ExpDecay(
            transform.rotation, yawTransform.rotation, smoothRotateSpeed, Time.deltaTime);
    }
}