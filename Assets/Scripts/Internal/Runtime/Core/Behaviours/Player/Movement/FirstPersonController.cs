using System;
using System.Collections;
using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Look;
using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement.Data;
using Assets.Scripts.Internal.Runtime.Core.Utilities;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour  //TODO: Change to LitTween
    {
        [Header("Data")]
        [SerializeField] HeadBobData headBobData;
        [Header("Locomotion Settings")]
        [SerializeField] float crouchSpeed = 1f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 3f;
        [SerializeField] float jumpSpeed = 5f;
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
        HeadBob headBob;
        CameraController cameraController;
        CharacterController characterController;
        Transform yawTransform;
        RaycastHit hitInfo;
        IEnumerator LandRoutine;
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
            InputManager.OnSprintPressed += OnSprintPressed;
            InputManager.OnSprintReleased += OnSprintReleased;
            InputManager.OnCrouchPressed += OnCrouchPressed;
            InputManager.OnCrouchReleased += OnCrouchReleased;
            InputManager.OnJumpPressed += OnJumpPressed;
        }

        void Start()
        {
            GetComponents();
            InitVariables();
        }

        void Update()
        {
            RotateTowardsCamera();
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
            InputManager.OnSprintPressed -= OnSprintPressed;
            InputManager.OnSprintReleased -= OnSprintReleased;
            InputManager.OnCrouchPressed -= OnCrouchPressed;
            InputManager.OnCrouchReleased -= OnCrouchReleased;
            InputManager.OnJumpPressed -= OnJumpPressed;
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

            var slideHeight = initHeight * slidePercent;
            slideCenter = (slideHeight / 2f + characterController.skinWidth) * Vector3.up;
            var slideStandHeightDifference = initHeight - slideHeight;
            slideCamHeight = initCamHeight - slideStandHeightDifference;

            finalRayLength = rayLength + characterController.center.y;

            isGrounded = true;
            previouslyGrounded = true;

            inAirTimer = 0f;
            headBob.CurrentStateHeight = initCamHeight;

            walkRunSpeedDifference = runSpeed - walkSpeed;
        }

        void SmoothInput() => smoothInputVector = Vector2.Lerp(
            smoothInputVector, InputManager.MovementAxis, Time.deltaTime * smoothInputSpeed);

        void SmoothSpeed()
        {
            smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, currentSpeed, Time.deltaTime * smoothVelocitySpeed);

            /* WALK TO RUN SPEED TRANSITION || COMMENTED BECAUSE RETURNING FROM SLIDING TO RUNNING IS TOO SNAPPY
            if (isRunning && CanRun() && !isSliding)
            {
                float walkRunPercent = Mathf.InverseLerp(walkSpeed,runSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(_walkRunPercent) * walkRunSpeedDifference + walkSpeed;
                return;
            }
            */

            finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }

        void SmoothDir() => smoothFinalMoveDir = Vector3.Lerp(
            smoothFinalMoveDir, finalMoveDir, Time.deltaTime * smoothFinalDirectionSpeed);

        void CheckIfGrounded()
        {
            var origin = transform.position + characterController.center;
            isGrounded = Physics.SphereCast(
                origin, raySphereRadius, Vector3.down, out hitInfo,
                finalRayLength, groundLayer);

            Debug.DrawRay(origin, Vector3.down * finalRayLength, isGrounded ? Color.red : Color.green);
        }

        void CheckIfWall()
        {
            var origin = transform.position + characterController.center;
            if (InputManager.MovementAxis != Vector2.zero && finalMoveDir.sqrMagnitude > 0)
                hitWall = Physics.SphereCast(
                    origin, rayObstacleSphereRadius, finalMoveDir,
                    out var _, rayObstacleLength, obstacleLayers);

            Debug.DrawRay(origin, finalMoveDir * rayObstacleLength, hitWall ? Color.red : Color.green);
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
            currentSpeed = InputManager.MovementAxis == Vector2.zero ? 0f : currentSpeed;
            currentSpeed = InputManager.MovementAxis.y == -1f ?
                currentSpeed * moveBackwardsSpeedPercent : currentSpeed;
            currentSpeed = InputManager.MovementAxis.x != 0f && InputManager.MovementAxis.y == 0f ?
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

            if (isRunning && !isCrouching && InputManager.MovementAxis != Vector2.zero && CanRun())
                HandleSlide();
            else
                HandleCrouch();
        }

        void HandleSlide()
        {
            isSliding = true;

            headBob.CurrentStateHeight = slideCamHeight;

            // DOTween.To(() => characterController.height,
            //     x => characterController.height = x, slideHeight, slideTransitionDuration)
            //     .SetEase(slideTransitionCurve);
            // DOTween.To(() => characterController.center,
            //     x => characterController.center = x, slideCenter, slideTransitionDuration)
            //     .SetEase(slideTransitionCurve);

            // yawTransform.DOLocalMoveY(slideCamHeight, slideTransitionDuration)
            //     .SetEase(slideTransitionCurve);

            this.CallWithDelay(ReturnToInitHeight, maxSlideDuration);
        }

        void ReturnToInitHeight()
        {
            if (CheckIfRoof())
            {
                //DOTween.Kill(this);
                isSliding = false;
                HandleCrouch();
                return;
            }

            if (!isSliding) return;

            //DOTween.Kill(this);

            isSliding = false;

            headBob.CurrentStateHeight = initCamHeight;

            // DOTween.To(() => characterController.height,
            //     x => characterController.height = x, initHeight, slideTransitionDuration)
            //         .SetEase(slideTransitionCurve);
            // DOTween.To(() => characterController.center,
            //     x => characterController.center = x, initCenter, slideTransitionDuration)
            //     .SetEase(slideTransitionCurve);

            // yawTransform.DOLocalMoveY(initCamHeight, slideTransitionDuration).SetEase(slideTransitionCurve);
        }

        void HandleCrouch()
        {
            if (isCrouching)
                if (CheckIfRoof())
                    return;

            if (LandRoutine != null) StopCoroutine(LandRoutine);

            duringCrouchAnimation = true;

            var currentHeight = characterController.height;
            var currentCenter = characterController.center;

            var desiredHeight = isCrouching ? initHeight : crouchHeight;
            var desiredCenter = isCrouching ? initCenter : crouchCenter;

            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = isCrouching ? initCamHeight : crouchCamHeight;

            isCrouching = !isCrouching;
            headBob.CurrentStateHeight = isCrouching ? crouchCamHeight : initCamHeight;

            // DOTween.To(() => characterController.height,
            //     x => characterController.height = x, desiredHeight, crouchTransitionDuration)
            //     .SetEase(crouchTransitionCurve);
            // DOTween.To(() => characterController.center,
            //     x => characterController.center = x, desiredCenter, crouchTransitionDuration)
            //     .SetEase(crouchTransitionCurve);

            // yawTransform.DOLocalMoveY(camDesiredHeight, crouchTransitionDuration)
            //     .SetEase(crouchTransitionCurve)
            //     .OnComplete(delegate { duringCrouchAnimation = false; });
        }

        void HandleLanding()
        {
            if (!previouslyGrounded && isGrounded)
                InvokeLandingRoutine();
        }

        void InvokeLandingRoutine()
        {
            if (LandRoutine != null) StopCoroutine(LandRoutine);

            LandRoutine = LandingRoutine();
            StartCoroutine(LandRoutine);
        }

        IEnumerator LandingRoutine()
        {
            var percent = 0f;
            var speed = 1f / landDuration;

            var localPos = yawTransform.localPosition;
            var initLandHeight = localPos.y;

            var landAmount = inAirTimer > landTimer ? highLandAmount : lowLandAmount;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var desiredY = landCurve.Evaluate(percent) * landAmount;

                localPos.y = initLandHeight + desiredY;
                yawTransform.localPosition = localPos;

                yield return null;
            }
        }

        void HandleHeadBob()
        {
            if (InputManager.MovementAxis != Vector2.zero && isGrounded && !hitWall)
            {
                if (!duringCrouchAnimation && !isSliding)
                {
                    headBob.ScrollHeadBob(isRunning && CanRun(), isCrouching, InputManager.MovementAxis);

                    yawTransform.localPosition = Vector3.Lerp(
                        yawTransform.localPosition,
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        Time.deltaTime * smoothHeadBobSpeed);
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
                        Time.deltaTime * smoothHeadBobSpeed);
            }

        }

        void HandleCameraSway() => cameraController.HandleSway(smoothInputVector, InputManager.MovementAxis.x);

        void HandleRunFOV()
        {
            if (!duringRunAnimation && InputManager.MovementAxis != Vector2.zero && !hitWall && isRunning && CanRun())
            {
                duringRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }

            if (duringRunAnimation && (InputManager.MovementAxis == Vector2.zero || !CanRun() || hitWall))
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }

        void ChangeToRunFOV()
        {
            if (!CanRun() || InputManager.MovementAxis == Vector2.zero)
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

        void RotateTowardsCamera() => transform.rotation = Quaternion.Slerp(
            transform.rotation, yawTransform.rotation, Time.deltaTime * smoothRotateSpeed);
    }
}