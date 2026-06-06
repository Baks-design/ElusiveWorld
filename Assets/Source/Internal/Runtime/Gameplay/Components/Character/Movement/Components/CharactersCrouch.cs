using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCrouch
    {
        readonly CharacterController controller;
        readonly CharactersChecks checks;
        readonly MovementSettings settings;
        readonly CharactersFlags flags;
        readonly InputManager input;
        float currentHeight;
        float targetHeight;

        public CharactersCrouch(
            MovementSettings settings,
            CharactersChecks checks,
            CharacterController controller,
            CharactersFlags flags,
            InputManager input)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.checks = checks ?? throw new ArgumentNullException(nameof(checks));
            this.controller = controller != null ? controller : throw new ArgumentNullException(nameof(controller));
            this.flags = flags ?? throw new ArgumentNullException(nameof(flags));
            this.input = input != null ? input : throw new ArgumentNullException(nameof(input));

            Initialize();
        }

        void Initialize()
        {
            flags.initHeight = controller.height;
            flags.initCenter = controller.center;

            flags.crouchHeight = flags.initHeight * settings.crouchPercent;
            flags.slideHeight = flags.initHeight * settings.slidePercent;

            currentHeight = controller.height;
            targetHeight = currentHeight;

            flags.crouchOffset = Vector3.zero;
        }

        public void OnCrouchPressed()
        {
            if (!checks.CanCrouch()) return;

            var canSlide =
                input.MovementAxis != Vector2.zero &&
                flags.isRunning &&
                !flags.isCrouching &&
                checks.CanRun();

            if (canSlide) StartSlide();
            else StartCrouch();
        }

        public void OnCrouchReleased() => StopCrouchOrSlide();

        public void Update()
        {
            UpdateState();
            UpdateHeight();
            UpdateCrouchOffset(); 
        }

        void UpdateState()
        {
            if (flags.isSliding)
            {
                targetHeight = flags.slideHeight;
                return;
            }

            targetHeight = flags.isCrouching ? flags.crouchHeight : flags.initHeight;
        }

        void UpdateHeight()
        {
            var newHeight = currentHeight.ExpDecay(targetHeight, settings.crouchSmooth, Time.deltaTime);
            if (newHeight > currentHeight && checks.CheckIfRoof()) return;

            currentHeight = newHeight;
            controller.height = currentHeight;
            controller.center = new Vector3(0f, currentHeight * 0.5f, 0f);
        }

        void UpdateCrouchOffset()
        {
            var heightDifference = flags.initHeight - currentHeight;
            flags.crouchOffset = new Vector3(0f, -heightDifference * 0.5f, 0f);
        }

        void StartCrouch()
        {
            if (flags.isCrouching) return;

            flags.isCrouching = true;
            flags.isSliding = false;
        }

        void StartSlide()
        {
            if (flags.isSliding) return;

            flags.isSliding = true;
            flags.isCrouching = false;
        }

        void StopCrouchOrSlide()
        {
            if (checks.CheckIfRoof()) return;

            flags.isCrouching = false;
            flags.isSliding = false;
        }
    }
}
