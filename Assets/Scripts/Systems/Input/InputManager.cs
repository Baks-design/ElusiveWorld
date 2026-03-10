using System;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Input
{
    public class InputManager : MonoBehaviour,
        IService,
        GameInputActions.IMovementActions,
        GameInputActions.ILookActions,
        GameInputActions.ICombatActions,
        GameInputActions.IInteractionActions,
        GameInputActions.IUIActions
    {
        GameInputActions input;

        public Vector2 MovementAxis { get; private set; }
        public Vector2 LookAxis { get; private set; }

        public event Action OnInteractPressed = delegate { };
        public event Action OnInteractReleased = delegate { };
        public event Action OnSprintPressed = delegate { };
        public event Action OnSprintReleased = delegate { };
        public event Action OnCrouchPressed = delegate { };
        public event Action OnCrouchReleased = delegate { };
        public event Action OnZoomPressed = delegate { };
        public event Action OnZoomReleased = delegate { };
        public event Action OnShootPressed = delegate { };
        public event Action OnShootHeld = delegate { };
        public event Action OnShootReleased = delegate { };
        public event Action OnReloadPressed = delegate { };
        public event Action OnJumpPressed = delegate { };

        public void Initialize()
        {
            IServiceLocator.Default.TryRegisterService(this);
            InputHelpers.ChangeCursorState(CursorLockMode.Locked);
            AddCallbacks();
        }

        public void Dispose()
        {
            IServiceLocator.Default.TryUnregisterService(this);
            RemoveCallbacks();
        }

        void AddCallbacks()
        {
            input ??= new GameInputActions();
            input.Movement.AddCallbacks(this);
            input.Look.AddCallbacks(this);
            input.Interaction.AddCallbacks(this);
            input.Combat.AddCallbacks(this);
            input.UI.AddCallbacks(this);
        }

        void RemoveCallbacks()
        {
            if (input == null) return;
            input.Disable();
            input.Movement.RemoveCallbacks(this);
            input.Look.RemoveCallbacks(this);
            input.Interaction.RemoveCallbacks(this);
            input.Combat.RemoveCallbacks(this);
            input.UI.RemoveCallbacks(this);
        }

        public void EnableGameplay()
        {
            input.Movement.Enable();
            input.Look.Enable();
            input.Combat.Enable();
            input.Interaction.Enable();
            input.UI.Disable();
        }

        public void EnableUI()
        {
            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.Interaction.Disable();
            input.UI.Enable();
        }

        public void DisableAllMaps()
        {
            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.UI.Disable();
            input.Interaction.Enable();
        }

        public void OnMove(CallbackContext context)
            => MovementAxis = context.ReadValue<Vector2>();

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

        public void OnLook(CallbackContext context)
            => LookAxis = context.ReadValue<Vector2>();

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
    }
}
