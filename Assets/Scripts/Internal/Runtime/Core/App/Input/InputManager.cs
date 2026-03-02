using System;
using Assets.Scripts.Internal.Runtime.Core.Utilities.Singletons;
using ElusiveWorld.Core;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    public class InputManager : Singleton<InputManager>,
        GameInputActions.IMovementActions,
        GameInputActions.ILookActions,
        GameInputActions.ICombatActions,
        GameInputActions.IInteractionActions,
        GameInputActions.IUIActions
    {
        GameInputActions playerInput;

        public static Vector2 MovementAxis { get; private set; }
        public static Vector2 LookAxis { get; private set; }

        public static event Action OnInteractPressed = delegate { };
        public static event Action OnInteractReleased = delegate { };
        public static event Action OnSprintPressed = delegate { };
        public static event Action OnSprintReleased = delegate { };
        public static event Action OnCrouchPressed = delegate { };
        public static event Action OnCrouchReleased = delegate { };
        public static event Action OnZoomPressed = delegate { };
        public static event Action OnZoomReleased = delegate { };
        public static event Action OnShootPressed = delegate { };
        public static event Action OnShootHeld = delegate { };
        public static event Action OnShootReleased = delegate { };
        public static event Action OnReloadPressed = delegate { };
        public static event Action OnJumpPressed = delegate { };

        protected override void Awake()
        {
            base.Awake();
            SetCallbacks();
        }

        void OnEnable() => EnableGameplay();

        void OnDisable() => DisableAllMaps();

        void OnDestroy() => playerInput.Dispose();

        void SetCallbacks()
        {
            playerInput ??= new GameInputActions();
            playerInput.Movement.SetCallbacks(this);
            playerInput.Look.SetCallbacks(this);
            playerInput.Interaction.SetCallbacks(this);
            playerInput.Combat.SetCallbacks(this);
            playerInput.UI.SetCallbacks(this);
        }

        public void EnableGameplay()
        {
            playerInput.Movement.Enable();
            playerInput.Look.Enable();
            playerInput.Combat.Enable();
            playerInput.Interaction.Enable();
            playerInput.UI.Disable();
        }

        public void EnableUI()
        {
            playerInput.Movement.Disable();
            playerInput.Look.Disable();
            playerInput.Combat.Disable();
            playerInput.Interaction.Disable();
            playerInput.UI.Enable();
        }

        public void DisableAllMaps()
        {
            playerInput.Movement.Disable();
            playerInput.Look.Disable();
            playerInput.Combat.Disable();
            playerInput.UI.Disable();
            playerInput.Interaction.Enable();
        }

        public void OnMove(CallbackContext context) => MovementAxis = context.ReadValue<Vector2>();
        public void OnSprint(CallbackContext context)
        {
            if (context.started) OnSprintPressed();
            if (context.canceled) OnSprintReleased();
        }
        public void OnCrouch(CallbackContext context)
        {
            if (context.started) OnCrouchPressed();
            if (context.canceled) OnCrouchReleased();
        }
        public void OnJump(CallbackContext context)
        {
            if (context.performed) OnJumpPressed();
        }

        public void OnLook(CallbackContext context) => LookAxis = context.ReadValue<Vector2>();
        public void OnAim(CallbackContext context)
        {
            if (context.started) OnZoomPressed();
            if (context.canceled) OnZoomReleased();
        }

        public void OnInteract(CallbackContext context)
        {
            if (context.started) OnInteractPressed();
            if (context.canceled) OnInteractReleased();
        }

        public void OnShoot(CallbackContext context)
        {
            if (context.started) OnShootPressed();
            if (context.canceled) OnShootReleased();
        }
        public void OnReload(CallbackContext context)
        {
            if (context.performed) OnReloadPressed();
        }

        public void OnNavigate(CallbackContext context) { }
        public void OnSubmit(CallbackContext context) { }
        public void OnCancel(CallbackContext context) { }
        public void OnPoint(CallbackContext context) { }
        public void OnClick(CallbackContext context) { }
        public void OnRightClick(CallbackContext context) { }
        public void OnMiddleClick(CallbackContext context) { }
        public void OnScrollWheel(CallbackContext context) { }
        public void OnTrackedDevicePosition(CallbackContext context) { }
        public void OnTrackedDeviceOrientation(CallbackContext context) { }
        public void OnTogglePauseMenu(CallbackContext context) { }
        public void OnShowLeaderboard(CallbackContext context) { }
        public void OnPause(CallbackContext context) { }
    }
}
