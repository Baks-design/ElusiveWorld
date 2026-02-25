using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Character
{
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerCharacter playerCharacter;
        [SerializeField] PlayerCamera playerCamera;
        [Space]
        [SerializeField] CameraSpring cameraSpring;
        [SerializeField] CameraLean cameraLean;
        [Space]
        [SerializeField] Volume volume;
        [SerializeField] StanceVignette stanceVignette;
        GameInputActions inputActions;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            inputActions = new GameInputActions();
            inputActions.Enable();

            playerCharacter.Initialize();
            playerCamera.Initialize(playerCharacter.GetCameraTarget());
            cameraSpring.Initialize();
            cameraLean.Initialize();
            stanceVignette.Initialize(volume.profile);
        }

        void Update()
        {
            var input = inputActions.Movement;
            var deltaTime = Time.deltaTime;
            var cameraTarget = playerCharacter.GetCameraTarget();
            var state = playerCharacter.GetState();

            var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
            playerCamera.UpdateRotation(cameraInput);
            playerCamera.UpdatePosition(cameraTarget);
            cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
            cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
            stanceVignette.UpdateVignette(deltaTime, state.Stance);

            var characterInput = new CharacterInput
            {
                Rotation = playerCamera.transform.rotation,
                Move = input.Move.ReadValue<Vector2>(),
                Jump = input.Jump.WasPressedThisFrame(),
                JumpSustain = input.Jump.IsPressed(),
                Crouch = input.Crouch.WasPressedThisFrame() ? CrouchInput.Toggle : CrouchInput.None
            };
            playerCharacter.UpdateInput(characterInput);
            playerCharacter.UpdateBody(deltaTime);

#if UNITY_EDITOR
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                if (Physics.Raycast(ray, out var hit))
                    Teleport(hit.point);
            }
#endif
        }

        public void Teleport(Vector3 position) => playerCharacter.SetPosition(position);

        void OnDestroy() => inputActions.Dispose();
    }
}
