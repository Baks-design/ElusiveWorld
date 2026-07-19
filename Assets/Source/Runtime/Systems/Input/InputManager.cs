using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
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
        [SerializeField, Range(0f, 1f)] float jumpBufferTime = 0.15f;
        [SerializeField, Range(0f, 1f)] float interactBufferTime = 0.12f;
        [SerializeField, Range(0f, 1f)] float shootBufferTime = 0.1f;
        [SerializeField, Range(0f, 1f)] float reloadBufferTime = 0.2f;
        [SerializeField, Range(0f, 1f)] float sprintBufferTime = 0.15f;
        [SerializeField, Range(0f, 1f)] float crouchBufferTime = 0.15f;
        readonly TimedInputBuffer<Action> jumpBuffer = new();
        readonly TimedInputBuffer<Action> interactBuffer = new();
        readonly TimedInputBuffer<Action> shootBuffer = new();
        readonly TimedInputBuffer<Action> reloadBuffer = new();
        readonly TimedInputBuffer<Action> sprintBuffer = new();
        readonly TimedInputBuffer<Action> crouchBuffer = new();
        GameInputActions input;

        public Vector2 MovementAxis { get; private set; }
        public Vector2 LookAxis { get; private set; }
        public bool IsSprintHeld { get; private set; }
        public bool IsCrouchHeld { get; private set; }
        public bool HasBufferedJump => jumpBuffer.HasBuffer;
        public bool HasBufferedInteract => interactBuffer.HasBuffer;
        public bool HasBufferedShoot => shootBuffer.HasBuffer;
        public bool HasBufferedReload => reloadBuffer.HasBuffer;
        public bool HasBufferedSprint => sprintBuffer.HasBuffer;
        public bool HasBufferedCrouch => crouchBuffer.HasBuffer;

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
        public event Action OnNextCharacterPressed = delegate { };
        public event Action OnPreviousCharacterPressed = delegate { };

        #region Setup
        public void Initialize()
        {
            CursorUtility.SetState(CursorLockMode.Locked, false);
            ConfigureBufferTimes();
            AddCallbacks();
        }

        void ConfigureBufferTimes()
        {
            jumpBuffer.SetHoldTime(jumpBufferTime);
            interactBuffer.SetHoldTime(interactBufferTime);
            shootBuffer.SetHoldTime(shootBufferTime);
            reloadBuffer.SetHoldTime(reloadBufferTime);
            sprintBuffer.SetHoldTime(sprintBufferTime);
            crouchBuffer.SetHoldTime(crouchBufferTime);
        }

        void AddCallbacks()
        {
            input = new GameInputActions();
            if (input == null) return;
            input.Movement.AddCallbacks(this);
            input.Look.AddCallbacks(this);
            input.Interaction.AddCallbacks(this);
            input.Combat.AddCallbacks(this);
            input.UI.AddCallbacks(this);
        }

        public void Dispose() => RemoveCallbacks();

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
        #endregion

        #region Maps
        public void EnableGameplay()
        {
            CursorUtility.SetState(CursorLockMode.Locked, false);

            input.Movement.Enable();
            input.Look.Enable();
            input.Combat.Enable();
            input.Interaction.Enable();
            input.UI.Disable();
        }

        public void EnableUI()
        {
            CursorUtility.SetState(CursorLockMode.None, true);

            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.Interaction.Disable();
            input.UI.Enable();
        }

        public void DisableAllMaps()
        {
            CursorUtility.SetState(CursorLockMode.Locked, false);

            input.Movement.Disable();
            input.Look.Disable();
            input.Combat.Disable();
            input.UI.Disable();
            input.Interaction.Enable();
        }
        #endregion

        #region Player
        public void OnMove(CallbackContext context)
            => MovementAxis = context.ReadValue<Vector2>();

        public void OnSprint(CallbackContext context)
        {
            if (context.started)
            {
                IsSprintHeld = true;
                OnSprintPressed();
                sprintBuffer.Set(() => OnSprintPressed());
            }
            if (context.canceled)
            {
                IsSprintHeld = false;
                OnSprintReleased();
                // Note: we don't clear the buffer on release — let it expire naturally
            }
        }
        public bool TryConsumeBufferedSprint()
        {
            if (sprintBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }

        public void OnCrouch(CallbackContext context)
        {
            if (context.started)
            {
                IsCrouchHeld = true;
                OnCrouchPressed();
                crouchBuffer.Set(() => OnCrouchPressed());
            }
            if (context.canceled)
            {
                IsCrouchHeld = false;
                OnCrouchReleased();
            }
        }
        public bool TryConsumeBufferedCrouch()
        {
            if (crouchBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }

        public void OnJump(CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpPressed();
                jumpBuffer.Set(() => OnJumpPressed());
            }
        }
        public bool TryConsumeBufferedJump()
        {
            if (jumpBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
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
            if (context.started)
            {
                OnInteractPressed();
                interactBuffer.Set(() => OnInteractPressed());
            }
            if (context.canceled) OnInteractReleased();
        }
        public bool TryConsumeBufferedInteract()
        {
            if (interactBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }

        public void OnNextCharacter(CallbackContext context)
        {
            if (context.started) OnNextCharacterPressed();
        }

        public void OnPreviousCharacter(CallbackContext context)
        {
            if (context.started) OnPreviousCharacterPressed();
        }

        public void OnShoot(CallbackContext context)
        {
            if (context.started)
            {
                OnShootPressed();
                shootBuffer.Set(() => OnShootPressed());
            }
            if (context.canceled) OnShootReleased();
        }
        public bool TryConsumeBufferedShoot()
        {
            if (shootBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }

        public void OnReload(CallbackContext context)
        {
            if (context.performed)
            {
                OnReloadPressed();
                reloadBuffer.Set(() => OnReloadPressed());
            }
        }
        public bool TryConsumeBufferedReload()
        {
            if (reloadBuffer.TryConsume(out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }
        #endregion

        #region UI
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
        #endregion
    }
}
