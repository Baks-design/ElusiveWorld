using UnityEngine;

namespace VHS
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] CameraInputData cameraInputData;
        [SerializeField] WeaponInputData weaponInputData;
        [SerializeField] MovementInputData movementInputData;
        [SerializeField] InteractionInputData interactionInputData;

        void Awake() => inputReader.Init();

        void OnEnable() //TODO: fIX iNPUT
        {
            inputReader.Look += look => cameraInputData.InputVector = look;
            inputReader.Aim += isAim => cameraInputData.IsZooming = isAim;
            inputReader.Move += movement => movementInputData.InputVector = movement;
            inputReader.Jump += isJump => movementInputData.IsJumping = isJump;
            inputReader.Crouch += isCrouch => movementInputData.IsCrouching = isCrouch;
            inputReader.Sprint += isSprint => movementInputData.IsRunning = isSprint;
            inputReader.Interact += isInteract => interactionInputData.IsInteracted = isInteract;
            inputReader.Shoot += isShoot => weaponInputData.IsShoot = isShoot;
            inputReader.Reload += isReload => weaponInputData.IsReload = isReload;
        }

        void OnDisable()
        {
            inputReader.Look -= look => cameraInputData.InputVector = look;
            inputReader.Aim -= isAim => cameraInputData.IsZooming = isAim;
            inputReader.Move -= movement => movementInputData.InputVector = movement;
            inputReader.Jump -= isJump => movementInputData.IsJumping = isJump;
            inputReader.Crouch -= isCrouch => movementInputData.IsCrouching = isCrouch;
            inputReader.Sprint -= isSprint => movementInputData.IsRunning = isSprint;
            inputReader.Interact -= isInteract => interactionInputData.IsInteracted = isInteract;
            inputReader.Shoot -= isShoot => weaponInputData.IsShoot = isShoot;
            inputReader.Reload -= isReload => weaponInputData.IsReload = isReload;
        }

        void OnDestroy() => inputReader.Dispose();
    }
}