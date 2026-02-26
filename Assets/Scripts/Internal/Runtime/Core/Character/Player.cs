using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Character
{
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerCharacter playerCharacter;
        [SerializeField] PlayerCamera playerCamera;
        [SerializeField] CameraSpring cameraSpring;
        [SerializeField] CameraLean cameraLean;
        [SerializeField] Volume volume;
        [SerializeField] StanceVignette stanceVignette;
        GameInputActions inputActions;

        public void Teleport(Vector3 position) => playerCharacter.SetPosition(position);

        void Start()
        {
            GetInput();
            InitializeComponents();
        }

        void GetInput()
        {
            inputActions = new GameInputActions();
            inputActions.Enable();
        }

        void InitializeComponents()
        {
            playerCharacter.Initialize();
            playerCamera.Initialize(playerCharacter.GetCameraTarget);
            cameraSpring.Initialize();
            cameraLean.Initialize();
            stanceVignette.Initialize(volume.profile);
        }

        void Update()
        {
            PlayerInput();
            EditorMethods();
        }

        void PlayerInput()
        {
            var input = inputActions.Movement;
            var deltaTime = Time.deltaTime;
            CameraInput(input, deltaTime);
            CharacterInput(input, deltaTime);
        }

        void CameraInput(GameInputActions.MovementActions input, float deltaTime)
        {
            var cameraTarget = playerCharacter.GetCameraTarget;
            var state = playerCharacter.GetState;
            var cameraInput = new CameraInput
            {
                Look = input.Look.ReadValue<Vector2>(),
                Aim = input.Aim.WasPressedThisFrame() || input.Aim.WasReleasedThisFrame()
            };

            playerCamera.UpdateRotation(cameraInput);
            playerCamera.UpdatePosition(cameraTarget);
            playerCamera.UpdateFOV(cameraInput);
            cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
            cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
            stanceVignette.UpdateVignette(deltaTime, state.Stance);
        }

        void CharacterInput(GameInputActions.MovementActions input, float deltaTime)
        {
            var characterInput = new CharacterInput
            {
                Rotation = playerCamera.transform.rotation,
                Move = input.Move.ReadValue<Vector2>(),
                Sprint = input.Sprint.IsPressed(),
                Jump = input.Jump.WasPressedThisFrame(),
                JumpSustain = input.Jump.IsPressed(),
                Crouch = input.Crouch.WasPressedThisFrame() ? CrouchInput.Toggle : CrouchInput.None
            };

            playerCharacter.UpdateInput(characterInput);
            playerCharacter.UpdateBody(deltaTime);
        }

#if UNITY_EDITOR
        void EditorMethods()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                if (Physics.Raycast(ray, out var hit))
                    Teleport(hit.point);
            }
        }
#endif

        void OnDestroy() => inputActions.Dispose();
    }
}
