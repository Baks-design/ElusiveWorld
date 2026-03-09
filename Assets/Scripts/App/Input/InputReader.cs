using ElusiveWorld.Core;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Data/Systems/InputReader")]
    public class InputReader : ScriptableObject,
        GameInputActions.IMovementActions,
        GameInputActions.ILookActions,
        GameInputActions.ICombatActions,
        GameInputActions.IInteractionActions,
        GameInputActions.IUIActions
    {
        GameInputActions input;

        public Vector2 MovementAxis { get; private set; }
        public Vector2 LookAxis { get; private set; }

        public event UnityAction OnInteractPressed = delegate { };
        public event UnityAction OnInteractReleased = delegate { };
        public event UnityAction OnSprintPressed = delegate { };
        public event UnityAction OnSprintReleased = delegate { };
        public event UnityAction OnCrouchPressed = delegate { };
        public event UnityAction OnCrouchReleased = delegate { };
        public event UnityAction OnZoomPressed = delegate { };
        public event UnityAction OnZoomReleased = delegate { };
        public event UnityAction OnShootPressed = delegate { };
        public event UnityAction OnShootHeld = delegate { };
        public event UnityAction OnShootReleased = delegate { };
        public event UnityAction OnReloadPressed = delegate { };
        public event UnityAction OnJumpPressed = delegate { };

        // Control
        public void Init()
        {
            input ??= new GameInputActions();
            input.Movement.AddCallbacks(this);
            input.Look.AddCallbacks(this);
            input.Interaction.AddCallbacks(this);
            input.Combat.AddCallbacks(this);
            input.UI.AddCallbacks(this);
        }
        public void Dispose()
        {
            input.Disable();
            input.Movement.RemoveCallbacks(this);
            input.Look.RemoveCallbacks(this);
            input.Interaction.RemoveCallbacks(this);
            input.Combat.RemoveCallbacks(this);
            input.UI.RemoveCallbacks(this);
        }

        // Maps
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

        // Movement
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

        // Look
        public void OnLook(CallbackContext context) => LookAxis = context.ReadValue<Vector2>();
        public void OnAim(CallbackContext context)
        {
            if (context.started) OnZoomPressed();
            if (context.canceled) OnZoomReleased();
        }

        // Interact
        public void OnInteract(CallbackContext context)
        {
            if (context.started) OnInteractPressed();
            if (context.canceled) OnInteractReleased();
        }

        // Combat
        public void OnShoot(CallbackContext context)
        {
            if (context.started) OnShootPressed();
            if (context.canceled) OnShootReleased();
        }
        public void OnReload(CallbackContext context)
        {
            if (context.performed) OnReloadPressed();
        }

        // UI
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
