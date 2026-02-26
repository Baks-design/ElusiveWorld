using Unity.Cinemachine;
using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] CinemachineCamera cam;
        [SerializeField] CameraMovement cameraMovement;
        [SerializeField] CameraZoom cameraZoom;

        public void Initialize(Transform target)
        {
            Cursor.lockState = CursorLockMode.Locked;
            cameraMovement.Initialize(transform, target);
            cameraZoom.Initialize(cam, this);
        }

        public void UpdateRotation(CameraInput input) => cameraMovement.UpdateRotation(input);

        public void UpdatePosition(Transform target) => cameraMovement.UpdatePosition(target);

        public void UpdateFOV(CameraInput input) { if (input.Aim) cameraZoom.ToggleZoom(); } //TODO: Check Aim

        public void UpdateRunFOV(bool returning) => cameraZoom.SetRunning(returning);
    }
}