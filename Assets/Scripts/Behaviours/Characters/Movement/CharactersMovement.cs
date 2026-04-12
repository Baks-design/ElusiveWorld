using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersMovement : MonoBehaviour, IUpdate
    {
        [SerializeField] CharacterController controller;
        [SerializeField] Transform yawTransform;
        [SerializeField] Transform camPivot;
        [SerializeField] CharactersLook look;
        [SerializeField] HeadBobData headBobData;
        [SerializeField] MovementSettings settings;
        readonly CharactersFlags flags = new();
        HeadBob headBob;
        InputManager input;
        CharactersCameraEffects effects;
        CharactersDisplacement displacement;
        CharactersChecks checks;
        CharactersCrouch crouch;

        void Awake()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            checks = new(flags, settings, controller, input);
            headBob = new(headBobData, settings.moveBackwardsSpeedPercent, settings.moveSideSpeedPercent);
            effects = new(this, flags, settings, checks, look, camPivot, headBob, input);
            crouch = new(this, settings, checks, controller, flags, camPivot, headBob, input);
            displacement = new(settings, checks, controller, flags, headBob, yawTransform, input);
        }

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);
            InputSubscribe();
        }

        void IUpdate.Update()
        {
            displacement.RotateTowardsCamera();
            checks.Update();
            displacement.UpdateProcess();
            effects.Update();
            displacement.UpdateVelocity();
            flags.previouslyGrounded = flags.isGrounded;
        }

        void OnDisable()
        {
            UpdateManager.UnregisterUpdate(this);
            InputUnsubscribe();
        }

        void InputSubscribe()
        {
            if (input == null) return;
            input.OnSprintPressed += effects.OnPlayerSprintPressed;
            input.OnSprintPressed += effects.OnPlayerSprintReleased;
            input.OnCrouchPressed += crouch.HandleCrouchInput;
            input.OnCrouchReleased += crouch.ReturnToInitHeight;
            input.OnJumpPressed += displacement.HandleJump;
        }
        void InputUnsubscribe()
        {
            if (input == null) return;
            input.OnSprintPressed -= effects.OnPlayerSprintPressed;
            input.OnSprintPressed -= effects.OnPlayerSprintReleased;
            input.OnCrouchPressed -= crouch.HandleCrouchInput;
            input.OnCrouchReleased -= crouch.ReturnToInitHeight;
            input.OnJumpPressed -= displacement.HandleJump;
        }
    }
}