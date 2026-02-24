using UnityEngine;
using UnityEngine.InputSystem;

namespace ElusiveWorld.Core.Character
{
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerCharacter playerCharacter;
        [SerializeField] PlayerCamera playerCamera;
        GameInputActions inputActions;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            inputActions = new GameInputActions();
            inputActions.Enable();

            playerCharacter.Initialize();
            playerCamera.Initialize(playerCharacter.GetCameraTarget());
        }

        void Update()
        {
            var input = inputActions.Movement;
            var deltaTime = Time.deltaTime;

            var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
            playerCamera.UpdateRotation(cameraInput);
            playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());

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
            if(Keyboard.current.tKey.wasPressedThisFrame)
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