using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Internal.Runtime.Systems.Physics;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    //TODO: Fix buffer inputs
    //TODO: Fix Buffer DeltaTime
    public class CharactersMovement : MonoBehaviour, IUpdate
    {
        [Header("References")]
        [SerializeField] CharacterController controller;
        [SerializeField] Transform yawTransform;
        [SerializeField] Transform camPivot;
        [SerializeField] CharactersLook look;
        [Header("Settings")]
        [SerializeField] HeadBobData headBobData;
        [SerializeField] MovementSettings settings;
        [SerializeField] RaycastConfig raycastConfig;
        [Header("Debug")]
        [SerializeField] bool showDebugInfo = true;
        readonly CharactersFlags flags = new();
        InputManager inputService;
        PhysicsService physicsService;
        HeadBob headBob;
        CharactersCameraEffects effects;
        CharactersDisplacement displacement;
        CharactersChecks checks;
        CharactersCrouch crouch;

        void Awake()
        {
            InitializeInputService();
            InitializePhysicsService();
            InitializeComponents();
            SetupStateMachine();
        }

        void InitializeInputService()
        {
            inputService = IServiceLocator.Default.GetService<InputManager>();
            if (inputService == null)
            {
                enabled = false;
                return;
            }
        }

        void InitializePhysicsService()
        {
            raycastConfig ??= new RaycastConfig();
            physicsService = controller.gameObject.CreatePhysicsService(raycastConfig);
            if (physicsService == null)
            {
                Debug.LogError("Failed to create physics service. " +
                    "Ensure GameObject has Rigidbody or CharacterController component.");
                enabled = false;
            }
        }

        void InitializeComponents()
        {
            checks = new(flags, settings, controller, inputService, physicsService);
            headBob = new(headBobData, settings.moveBackwardsSpeedPercent, settings.moveSideSpeedPercent);
            effects = new(settings, flags, checks, look, camPivot, headBob, inputService);
            crouch = new(settings, checks, controller, flags, inputService);
            displacement = new(settings, checks, controller, flags, headBob, yawTransform, inputService, physicsService);
        }

        void SetupStateMachine() { }

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);
            InputSubscribe();
        }

        void InputSubscribe() 
        {
            if (inputService == null) return;
            inputService.OnSprintPressed += effects.OnPlayerSprintPressed;
            inputService.OnSprintReleased += effects.OnPlayerSprintReleased;
            inputService.OnCrouchPressed += crouch.OnCrouchPressed;
            inputService.OnCrouchReleased += crouch.OnCrouchReleased;
            inputService.OnJumpPressed += displacement.HandleJump;
        }

        void IUpdate.Update()
        {
            displacement.RotateTowardsCamera();
            checks.Update();
            displacement.UpdateProcess();
            crouch.Update();
            effects.Update();
            displacement.UpdateVelocity();
            flags.previouslyGrounded = flags.isGrounded;
            if (showDebugInfo) DrawDebugInfo();
        }

        void DrawDebugInfo()
        {
            // GUI debug info
            if (showDebugInfo)
            {
                var pos = transform.position;
                Debug.DrawLine(pos, pos + Vector3.down * 0.5f, physicsService.IsGrounded ? Color.green : Color.red);

                if (physicsService.IsOnSlope)
                    Debug.DrawLine(pos, pos + physicsService.Raycast.GroundNormal * 2f, Color.yellow);
            }
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw raycast gizmos in the editor
            if (Application.isEditor)
            {
                switch (physicsService.Raycast)
                {
                    case RigidbodyRaycastService rbRaycast: rbRaycast.DrawGizmos(); break;
                    case CharacterControllerRaycastService ccRaycast: ccRaycast.DrawGizmos(); break;
                }
            }
        }

        void OnDisable()
        {
            InputUnsubscribe();
            UpdateManager.UnregisterUpdate(this);
        }

        void InputUnsubscribe()
        {
            if (inputService == null) return;
            inputService.OnSprintPressed -= effects.OnPlayerSprintPressed;
            inputService.OnSprintReleased -= effects.OnPlayerSprintReleased;
            inputService.OnCrouchPressed -= crouch.OnCrouchPressed;
            inputService.OnCrouchReleased -= crouch.OnCrouchReleased;
            inputService.OnJumpPressed -= displacement.HandleJump;
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10f, 10f, 300f, 200f));
            GUILayout.Label("Physics Debug Info");
            GUILayout.Label($"Grounded: {physicsService.IsGrounded}");
            GUILayout.Label($"On Slope: {physicsService.IsOnSlope}");
            GUILayout.Label($"Slope Angle: {physicsService.CurrentSlopeAngle:F1}°");
            GUILayout.Label($"Can Stand: {physicsService.CanStandUp()}");
            GUILayout.EndArea();
        }
    }
}