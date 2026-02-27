using Unity.Cinemachine;
using UnityEngine;

namespace VHS
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] CinemachineCamera cam;
        [Header("Data")]
        [SerializeField] CameraInputData camInputData;
        [Header("Custom Classes")]
        [SerializeField] CameraMovement cameraMovement;
        [SerializeField] CameraZoom cameraZoom;
        [SerializeField] CameraSwaying cameraSway;

        void Start()
        {
            GetComponents();
            InitComponents();
            ChangeCursorState();
        }

        void GetComponents() => cam = GetComponentInChildren<CinemachineCamera>();

        void InitComponents()
        {
            cameraMovement.Init(transform, camInputData);
            cameraZoom.Init(cam, camInputData);
            cameraSway.Init(cam.transform);
        }

        void ChangeCursorState() => Cursor.lockState = CursorLockMode.Locked;

        void LateUpdate()
        {
            HandleMovement();
            HandleZoom();
        }

        void HandleMovement() => cameraMovement.Update();

        void HandleZoom()
        {
            if (camInputData.ZoomClicked || camInputData.ZoomReleased)
                cameraZoom.ChangeFOV(this);
        }

        public void HandleSway(Vector3 inputVector, float rawXInput) =>
            cameraSway.SwayPlayer(inputVector, rawXInput);

        public void ChangeRunFOV(bool returning) => cameraZoom.ChangeRunFOV(returning, this);
    }
}
