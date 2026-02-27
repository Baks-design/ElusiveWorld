using UnityEngine;
using ElusiveWorld.Core;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static ElusiveWorld.Core.GameInputActions;
using static UnityEngine.InputSystem.InputAction;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/InputReader")]
    public class InputReader : ScriptableObject,
        IMovementActions,
        ILookActions,
        IInteractionActions,
        ICombatActions,
        IUIActions
    {
        GameInputActions input;

        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2> Look = delegate { };
        public event UnityAction<bool> Aim = delegate { };
        public event UnityAction<bool> Sprint = delegate { };
        public event UnityAction<bool> Crouch = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Interact = delegate { };
        public event UnityAction<bool> Shoot = delegate { };
        public event UnityAction<bool> Reload = delegate { };

        public void Init()
        {
            if (input != null) return;
            input = new GameInputActions();
            input.Movement.SetCallbacks(this);
            input.Look.SetCallbacks(this);
            input.Interaction.SetCallbacks(this);
            input.Combat.SetCallbacks(this);
            input.UI.SetCallbacks(this);
            input.Enable();
        }

        public void Dispose()
        {
            if (input == null) return;
            input.Disable();
            input.Movement.RemoveCallbacks(this);
            input.Look.RemoveCallbacks(this);
            input.Interaction.RemoveCallbacks(this);
            input.Combat.RemoveCallbacks(this);
            input.UI.RemoveCallbacks(this);
            input.Dispose();
        }

        // Maps
        public void EnableGameplayInput()
        {
            input.Movement.Enable();
            input.Look.Enable();
            input.Combat.Enable();
            input.Interaction.Enable();
            input.UI.Disable();
        }

        public void EnableUIInput()
        {
            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.Interaction.Disable();
            input.UI.Enable();
        }

        public void DisableAllInput()
        {
            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.Interaction.Disable();
            input.UI.Disable();
        }

        // Look
        public void OnLook(CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>());

        public void OnAim(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Aim.Invoke(true); break;
                case InputActionPhase.Canceled: Aim.Invoke(false); break;
            }
        }

        // Interact
        public void OnInteract(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Interact.Invoke(true); break;
                case InputActionPhase.Canceled: Interact.Invoke(false); break;
            }
        }

        // Movement
        public void OnMove(CallbackContext context) => Move.Invoke(context.ReadValue<Vector2>());

        public void OnCrouch(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Crouch.Invoke(true); break;
                case InputActionPhase.Canceled: Crouch.Invoke(false); break;
            }
        }

        public void OnJump(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Jump.Invoke(true); break;
                case InputActionPhase.Canceled: Jump.Invoke(false); break;
            }
        }

        public void OnSprint(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Sprint.Invoke(true); break;
                case InputActionPhase.Canceled: Sprint.Invoke(false); break;
            }
        }

        // Combat
        public void OnShoot(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Shoot.Invoke(true); break;
                case InputActionPhase.Canceled: Shoot.Invoke(false); break;
            }
        }

        public void OnReload(CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started: Reload.Invoke(true); break;
                case InputActionPhase.Canceled: Reload.Invoke(false); break;
            }
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