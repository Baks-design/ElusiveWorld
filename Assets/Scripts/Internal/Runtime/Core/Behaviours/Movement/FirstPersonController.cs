using System.Collections;
using UnityEngine;

namespace VHS
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] MovementInputData movementInputData;
        [SerializeField] HeadBobData headBobData;
        [Header("Locomotion Settings")]
        [SerializeField] float crouchSpeed = 1f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 3f;
        [SerializeField] float jumpSpeed = 5f;
        [SerializeField, Range(0f, 1f)] float moveBackwardsSpeedPercent = 0.5f;
        [SerializeField, Range(0f, 1f)] float moveSideSpeedPercent = 0.75f;
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
        HeadBob headBob;
        CameraController cameraController;
        CharacterController characterController;
        Transform yawTransform;
        RaycastHit hitInfo;
        IEnumerator crouchRoutine;
        IEnumerator landRoutine;
        Vector3 finalMoveDir;
        Vector3 smoothFinalMoveDir;
        Vector3 finalMoveVector;
        Vector3 initCenter;
        Vector3 crouchCenter;
        Vector2 inputVector;
        Vector2 smoothInputVector;
        float currentSpeed;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;
        float walkRunSpeedDifference;
        float finalRayLength;
        float initHeight;
        float crouchHeight;
        float initCamHeight;
        float crouchCamHeight;
        float crouchStandHeightDifference;
        float inAirTimer;
        bool hitWall;
        bool isGrounded;
        bool previouslyGrounded;
        bool duringCrouchAnimation;
        bool duringRunAnimation;

        void Start()
        {
            GetComponents();
            InitVariables();
        }

        void GetComponents()
        {
            characterController = GetComponent<CharacterController>();
            cameraController = GetComponentInChildren<CameraController>();
            yawTransform = cameraController.transform;
            headBob = new HeadBob(headBobData, moveBackwardsSpeedPercent, moveSideSpeedPercent);
        }

        void InitVariables()
        {
            characterController.center = new Vector3(0f, characterController.height / 2f + characterController.skinWidth, 0f);

            initCenter = characterController.center;
            initHeight = characterController.height;
            crouchHeight = initHeight * crouchPercent;
            crouchCenter = (crouchHeight / 2f + characterController.skinWidth) * Vector3.up;
            crouchStandHeightDifference = initHeight - crouchHeight;
            initCamHeight = yawTransform.localPosition.y;
            crouchCamHeight = initCamHeight - crouchStandHeightDifference;

            finalRayLength = rayLength + characterController.center.y;

            isGrounded = true;
            previouslyGrounded = true;
            inAirTimer = 0f;

            headBob.CurrentStateHeight = initCamHeight;

            walkRunSpeedDifference = runSpeed - walkSpeed;
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
            HandleCrouch();
            HandleHeadBob();
            HandleRunFOV();
            HandleCameraSway();
            HandleLanding();
            ApplyGravity();
            ApplyMovement();
            previouslyGrounded = isGrounded;
        }

        void SmoothInput()
        {
            inputVector = movementInputData.InputVector.normalized;
            smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime * smoothInputSpeed);
        }

        void SmoothSpeed()
        {
            smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, currentSpeed, Time.deltaTime * smoothVelocitySpeed);

            if (movementInputData.IsRunning && CanRun())
            {
                var walkRunPercent = Mathf.InverseLerp(walkSpeed, runSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(walkRunPercent) * walkRunSpeedDifference + walkSpeed;
            }
            else
                finalSmoothCurrentSpeed = smoothCurrentSpeed;
        }

        void SmoothDir() => smoothFinalMoveDir = Vector3.Lerp(smoothFinalMoveDir, finalMoveDir, Time.deltaTime * smoothFinalDirectionSpeed);

        void CheckIfGrounded()
        {
            var origin = transform.position + characterController.center;
            var hitGround = Physics.SphereCast(origin, raySphereRadius, Vector3.down, out hitInfo, finalRayLength, groundLayer);
            //Debug.DrawRay(origin, Vector3.down * finalRayLength, Color.red);
            isGrounded = hitGround;
        }

        void CheckIfWall()
        {
            var origin = transform.position + characterController.center;
            var isHitWall = false;
            if (movementInputData.HasInput && finalMoveDir.sqrMagnitude > 0)
                isHitWall = Physics.SphereCast(
                    origin, rayObstacleSphereRadius, finalMoveDir, out var _wallInfo, rayObstacleLength, obstacleLayers);
            //Debug.DrawRay(origin, finalMoveDir * rayObstacleLength, Color.blue);
            hitWall = isHitWall;
        }

        bool CheckIfRoof()
        {
            var origin = transform.position;
            var hitRoof = Physics.SphereCast(origin, raySphereRadius, Vector3.up, out var _roofInfo, initHeight);
            return hitRoof;
        }

        bool CanRun()
        {
            var normalizedDir = Vector3.zero;
            if (smoothFinalMoveDir != Vector3.zero)
                normalizedDir = smoothFinalMoveDir.normalized;
            var dot = Vector3.Dot(transform.forward, normalizedDir);
            return dot >= canRunThreshold && !movementInputData.IsCrouching;
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
            if (isGrounded)
                vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, hitInfo.normal);
            return vectorToFlat;
        }

        void CalculateSpeed()
        {
            currentSpeed = movementInputData.IsRunning && CanRun() ? runSpeed : walkSpeed;
            currentSpeed = movementInputData.IsCrouching ? crouchSpeed : currentSpeed;
            currentSpeed = !movementInputData.HasInput ? 0f : currentSpeed;
            currentSpeed = movementInputData.InputVector.y == -1
                ? currentSpeed * moveBackwardsSpeedPercent : currentSpeed;
            currentSpeed = movementInputData.InputVector.x != 0 && movementInputData.InputVector.y == 0
                ? currentSpeed * moveSideSpeedPercent : currentSpeed;
        }

        void CalculateFinalMovement()
        {
            var finalVector = smoothFinalMoveDir * finalSmoothCurrentSpeed;
            finalMoveVector.x = finalVector.x;
            finalMoveVector.z = finalVector.z;
            if (characterController.isGrounded)
                finalMoveVector.y += finalVector.y;
        }

        void HandleCrouch()
        {
            if (movementInputData.IsCrouching && isGrounded)
                InvokeCrouchRoutine();
        }

        void InvokeCrouchRoutine()
        {
            if (movementInputData.IsCrouching)
                if (CheckIfRoof())
                    return;

            if (landRoutine != null) StopCoroutine(landRoutine);
            if (crouchRoutine != null) StopCoroutine(crouchRoutine);

            crouchRoutine = CrouchRoutine();
            StartCoroutine(crouchRoutine);
        }

        IEnumerator CrouchRoutine()
        {
            duringCrouchAnimation = true;

            var percent = 0f;
            var speed = 1f / crouchTransitionDuration;

            var currentHeight = characterController.height;
            var currentCenter = characterController.center;

            var desiredHeight = movementInputData.IsCrouching ? initHeight : crouchHeight;
            var desiredCenter = movementInputData.IsCrouching ? initCenter : crouchCenter;

            var camPos = yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = movementInputData.IsCrouching ? initCamHeight : crouchCamHeight;

            movementInputData.IsCrouching = !movementInputData.IsCrouching;
            headBob.CurrentStateHeight = movementInputData.IsCrouching ? crouchCamHeight : initCamHeight;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = crouchTransitionCurve.Evaluate(percent);

                characterController.height = Mathf.Lerp(currentHeight, desiredHeight, smoothPercent);
                characterController.center = Vector3.Lerp(currentCenter, desiredCenter, smoothPercent);

                camPos.y = Mathf.Lerp(camCurrentHeight, camDesiredHeight, smoothPercent);
                yawTransform.localPosition = camPos;

                yield return null;
            }

            duringCrouchAnimation = false;
        }

        void HandleLanding()
        {
            if (!previouslyGrounded && isGrounded)
                InvokeLandingRoutine();
        }

        void InvokeLandingRoutine()
        {
            if (landRoutine != null) StopCoroutine(landRoutine);

            landRoutine = LandingRoutine();
            StartCoroutine(landRoutine);
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
            if (movementInputData.HasInput && isGrounded && !hitWall)
            {
                if (!duringCrouchAnimation)
                {
                    headBob.ScrollHeadBob(
                        movementInputData.IsRunning && CanRun(),
                        movementInputData.IsCrouching,
                        movementInputData.InputVector);

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

        void HandleCameraSway() => cameraController.HandleSway(smoothInputVector, movementInputData.InputVector.x);

        void HandleRunFOV()
        {
            if (movementInputData.HasInput && isGrounded && !hitWall)
            {
                if (movementInputData.IsRunning && CanRun())
                {
                    duringRunAnimation = true;
                    cameraController.ChangeRunFOV(false);
                }

                if (movementInputData.IsRunning && CanRun() && !duringRunAnimation)
                {
                    duringRunAnimation = true;
                    cameraController.ChangeRunFOV(false);
                }
            }

            if ((movementInputData.IsRunning || !movementInputData.HasInput || hitWall) && duringRunAnimation)
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }
        void HandleJump()
        {
            if (movementInputData.IsJumping && !movementInputData.IsCrouching)
            {
                finalMoveVector.y = jumpSpeed;
                previouslyGrounded = true;
                isGrounded = false;
            }
        }

        void ApplyGravity()
        {
            if (characterController.isGrounded)
            {
                inAirTimer = 0f;
                finalMoveVector.y = -stickToGroundForce;
                HandleJump();
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
