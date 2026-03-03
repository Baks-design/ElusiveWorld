using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Look
{
    public class CameraController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] InputReader input;
        [Header("Look Settings")]
        [SerializeField] Vector2 sensitivity = Vector2.zero;
        [SerializeField] Vector2 smoothAmount = Vector2.zero;
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 lookAngleMinMax = Vector2.zero;
        [Header("Custom Classes")]
        [SerializeField] CameraZoom cameraZoom;
        [SerializeField] CameraSwaying cameraSway;
        Transform pitchTranform;
        CinemachineCamera cam;
        float yaw;
        float pitch;
        float desiredYaw;
        float desiredPitch;

        void Awake()
        {
            GetComponents();
            InitValues();
            InitComponents();
            ChangeCursorState();
        }

        void OnEnable()
        {
            input.OnZoomPressed += OnZoomPressed;
            input.OnZoomReleased += OnZoomReleased;
        }

        void LateUpdate()
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
        }

        void OnDisable()
        {
            input.OnZoomPressed -= OnZoomPressed;
            input.OnZoomReleased -= OnZoomReleased;
        }

        static void ChangeCursorState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

        void OnZoomPressed() => cameraZoom.ChangeFOV(this);

        void OnZoomReleased() => cameraZoom.ChangeFOV(this);

        void CalculateRotation()
        {
            desiredYaw += input.LookAxis.x * sensitivity.x * Time.deltaTime;
            desiredPitch -= input.LookAxis.y * sensitivity.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, lookAngleMinMax.x, lookAngleMinMax.y);
        }

        void SmoothRotation()
        {
            yaw = Mathf.Lerp(yaw, desiredYaw, smoothAmount.x * Time.deltaTime);
            pitch = Mathf.Lerp(pitch, desiredPitch, smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation()
        {
            transform.eulerAngles = new Vector3(0f, yaw, 0f);
            pitchTranform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        public void HandleSway(Vector3 inputVector, float rawXInput) => cameraSway.SwayPlayer(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning) => cameraZoom.ChangeRunFOV(returning, this);
    }
}
