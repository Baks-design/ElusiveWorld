using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Look
{
    public class CameraController : MonoBehaviour, ILateUpdate
    {
        [Header("Look Settings")]
        [SerializeField] Vector2 sensitivity = Vector2.zero;
        [SerializeField] Vector2 smoothAmount = Vector2.zero;
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 lookAngleMinMax = Vector2.zero;
        [Header("Custom Classes")]
        [SerializeField] CameraZoom cameraZoom;
        [SerializeField] CameraSwaying cameraSway;
        InputManager input;
        Transform pitchTranform;
        CinemachineCamera cam;
        Quaternion finalYaw;
        Quaternion finalPitch;
        Quaternion targetYaw;
        Quaternion targetPitch;
        float yaw;
        float desiredYaw;
        float desiredPitch;

        void Start()
        {
            UpdateManager.RegisterLateUpdate(this);

            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnZoomPressed += OnZoomPressed;
            input.OnZoomReleased += OnZoomReleased;

            GetComponents();
            InitValues();
            InitComponents();
        }

        void ILateUpdate.LateUpdate()
        {
            CalculateRotation();
            PassRotation();
            SmoothRotation();
            ApplyRotation();
        }

        void OnDisable()
        {
            UpdateManager.UnregisterLateUpdate(this);

            input.OnZoomPressed -= OnZoomPressed;
            input.OnZoomReleased -= OnZoomReleased;
        }

        void GetComponents()
        {
            pitchTranform = transform.GetChild(0).transform;
            cam = GetComponentInChildren<CinemachineCamera>();
        }

        void InitValues()
        {
            yaw = transform.eulerAngles.y;
            desiredYaw = yaw;
        }

        void InitComponents()
        {
            cameraZoom.Init(cam);
            cameraSway.Init(cam.transform);
        }

        void OnZoomPressed() => _ = cameraZoom.ChangeFOV();

        void OnZoomReleased() => _ = cameraZoom.ChangeFOV();

        void CalculateRotation()
        {
            desiredYaw += input.LookAxis.x * sensitivity.x * Time.deltaTime;
            desiredPitch -= input.LookAxis.y * sensitivity.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, lookAngleMinMax.x, lookAngleMinMax.y);
        }

        void PassRotation()
        {
            targetYaw = Quaternion.Euler(0f, desiredYaw, 0f);
            targetPitch = Quaternion.Euler(desiredPitch, 0f, 0f);
        }

        void SmoothRotation()
        {
            var deltaTime = Time.deltaTime;
            finalYaw = finalYaw.ExpDecay(targetYaw, smoothAmount.x, deltaTime);
            finalPitch = finalPitch.ExpDecay(targetPitch, smoothAmount.y, deltaTime);
        }

        void ApplyRotation()
        {
            transform.rotation = finalYaw;
            pitchTranform.localRotation = finalPitch;
        }

        public void HandleSway(Vector3 inputVector, float rawXInput) => cameraSway.SwayPlayer(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning) => _ = cameraZoom.ChangeRunFOV(returning);
    }
}
