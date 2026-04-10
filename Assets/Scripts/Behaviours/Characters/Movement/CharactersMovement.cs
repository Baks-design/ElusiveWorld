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
            controller.center = new(0f, controller.height / 2f + controller.skinWidth, 0f);
            headBob = new(headBobData, settings.moveBackwardsSpeedPercent, settings.moveSideSpeedPercent);
            checks = new(flags, settings, controller);
            effects = new(flags, settings, checks, look, camPivot, headBob);
            crouch = new(settings, checks, controller, flags, camPivot, headBob, effects);
            displacement = new(settings, checks, controller, flags, headBob, yawTransform);
        }

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);

            input = IServiceLocator.Default.GetService<InputManager>();
            if (input != null)
            {
                input.OnSprintPressed += OnSprintPressed;
                input.OnSprintPressed += effects.OnPlayerSprintReleased;
                input.OnCrouchPressed += OnCrouchPressed;
                input.OnCrouchReleased += OnCrouchReleased;
                input.OnJumpPressed += displacement.HandleJump;
            }
        }

        void IUpdate.Update()
        {
            displacement.RotateTowardsCamera();
            checks.Update(input);
            displacement.UpdateProcess(input);
            effects.Update(this, input);
            displacement.UpdateVelocity();
            flags.previouslyGrounded = flags.isGrounded;
        }

        void OnDisable()
        {
            UpdateManager.UnregisterUpdate(this);

            if (input != null)
            {
                input.OnSprintPressed -= OnSprintPressed;
                input.OnSprintPressed -= effects.OnPlayerSprintReleased;
                input.OnCrouchPressed -= OnCrouchPressed;
                input.OnCrouchReleased -= OnCrouchReleased;
                input.OnJumpPressed -= displacement.HandleJump;
            }
        }

        void OnSprintPressed() => effects.OnPlayerSprintPressed(input);
        void OnCrouchPressed() => crouch.HandleCrouchInput(this, input);
        void OnCrouchReleased() => crouch.ReturnToInitHeight(this);
    }
}