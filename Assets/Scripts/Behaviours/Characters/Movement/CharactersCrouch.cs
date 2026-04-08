using UnityEngine;
using LitMotion;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCrouch //FIXME: TASK
    {
        readonly CharacterController controller;
        readonly Transform camPivot;
        readonly HeadBob headBob;
        readonly MovementSettings settings;
        readonly CharactersChecks checks;
        readonly CharactersFlags flags;
        readonly CharactersCameraEffects effects;
        MotionHandle slideHeightHandle;
        MotionHandle slideCenterHandle;
        MotionHandle slideCamHandle;
        MotionHandle slideReturnHandle;
        MotionHandle crouchHeightHandle;
        MotionHandle crouchCenterHandle;
        MotionHandle crouchCamHandle;

        public CharactersCrouch(
            MovementSettings settings,
            CharactersChecks checks,
            CharacterController controller,
            CharactersFlags flags,
            Transform camPivot,
            HeadBob headBob,
            CharactersCameraEffects effects)
        {
            this.settings = settings;
            this.checks = checks;
            this.controller = controller;
            this.flags = flags;
            this.headBob = headBob;
            this.camPivot = camPivot;
            this.effects = effects;

            flags.initCenter = controller.center;
            flags.initHeight = controller.height;
            flags.initCamHeight = camPivot.localPosition.y;

            flags.crouchHeight = flags.initHeight * settings.crouchPercent;
            flags.crouchCenter = (flags.crouchHeight / 2f + controller.skinWidth) * Vector3.up;
            flags.crouchStandHeightDifference = flags.initHeight - flags.crouchHeight;
            flags.crouchCamHeight = flags.initCamHeight - flags.crouchStandHeightDifference;

            flags.slideHeight = flags.initHeight * settings.slidePercent;
            flags.slideCenter = (flags.slideHeight / 2f + controller.skinWidth) * Vector3.up;
            flags.slideStandHeightDifference = flags.initHeight - flags.slideHeight;
            flags.slideCamHeight = flags.initCamHeight - flags.slideStandHeightDifference;
        }

        public void HandleCrouchInput()
        {
            if (!checks.CanCrouch()) return;

            if (flags.isRunning
                && !flags.isCrouching
                && flags.finalMoveVector.sqrMagnitude > 0.1f
                && checks.CanRun())
                HandleSlide();
            else
                HandleCrouch();
        }

        void HandleSlide()
        {
            flags.isSliding = true;
            flags.duringSlideAnimation = true;

            headBob.CurrentStateHeight = flags.slideCamHeight;

            // Cancel any existing motion handles
            CancelSlideMotions();

            // Animate CharacterController height
            slideHeightHandle = LMotion.Create(controller.height, flags.slideHeight, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(x => controller.height = x);

            // Animate CharacterController center
            slideCenterHandle = LMotion.Create(controller.center, flags.slideCenter, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(x => controller.center = x);

            // Animate camera pivot
            slideCamHandle = LMotion.Create(camPivot.localPosition.y, flags.slideCamHeight, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(y => camPivot.localPosition = new Vector3(camPivot.localPosition.x, y, camPivot.localPosition.z));

            // Schedule return to initial height
            slideReturnHandle = LMotion.Create(0f, 1f, settings.maxSlideDuration)
                .WithOnComplete(() => ReturnToInitHeight())
                .Bind(_ => { });
        }

        void CancelSlideMotions()
        {
            if (slideHeightHandle.IsActive()) slideHeightHandle.Cancel();
            if (slideCenterHandle.IsActive()) slideCenterHandle.Cancel();
            if (slideCamHandle.IsActive()) slideCamHandle.Cancel();
            if (slideReturnHandle.IsActive()) slideReturnHandle.Cancel();
        }

        public void ReturnToInitHeight()
        {
            if (checks.CheckIfRoof())
            {
                CancelSlideMotions();
                flags.isSliding = false;
                HandleCrouch();
                return;
            }

            if (!flags.isSliding) return;

            CancelSlideMotions();
            flags.isSliding = false;
            flags.duringSlideAnimation = false;

            headBob.CurrentStateHeight = flags.initCamHeight;

            // Return to initial height
            LMotion.Create(controller.height, flags.initHeight, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(x => controller.height = x);

            // Return to initial center
            LMotion.Create(controller.center, flags.initCenter, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(x => controller.center = x);

            // Return camera to initial height
            LMotion.Create(camPivot.localPosition.y, flags.initCamHeight, settings.slideTransitionDuration)
                .WithEase(settings.slideTransitionCurve)
                .Bind(y => camPivot.localPosition = new Vector3(camPivot.localPosition.x, y, camPivot.localPosition.z));
        }

        void HandleCrouch()
        {
            if (flags.isCrouching && checks.CheckIfRoof()) return;

            effects.landingTokenSource?.Cancel();
            effects.landingTokenSource?.Dispose();

            flags.duringCrouchAnimation = true;

            var desiredHeight = flags.isCrouching ? flags.initHeight : flags.crouchHeight;
            var desiredCenter = flags.isCrouching ? flags.initCenter : flags.crouchCenter;
            var camDesiredHeight = flags.isCrouching ? flags.initCamHeight : flags.crouchCamHeight;

            flags.isCrouching = !flags.isCrouching;
            headBob.CurrentStateHeight = flags.isCrouching ? flags.crouchCamHeight : flags.initCamHeight;

            // Cancel any existing crouch motions
            CancelCrouchMotions();

            // Animate CharacterController height
            crouchHeightHandle = LMotion.Create(controller.height, desiredHeight, settings.crouchTransitionDuration)
                .WithEase(settings.crouchTransitionCurve)
                .Bind(x => controller.height = x);

            // Animate CharacterController center
            crouchCenterHandle = LMotion.Create(controller.center, desiredCenter, settings.crouchTransitionDuration)
                .WithEase(settings.crouchTransitionCurve)
                .Bind(x => controller.center = x);

            // Animate camera pivot with completion callback
            crouchCamHandle = LMotion.Create(camPivot.localPosition.y, camDesiredHeight, settings.crouchTransitionDuration)
                .WithEase(settings.crouchTransitionCurve)
                .WithOnComplete(() => flags.duringCrouchAnimation = false)
                .Bind(y => camPivot.localPosition = new Vector3(camPivot.localPosition.x, y, camPivot.localPosition.z));
        }

        void CancelCrouchMotions()
        {
            if (crouchHeightHandle.IsActive()) crouchHeightHandle.Cancel();
            if (crouchCenterHandle.IsActive()) crouchCenterHandle.Cancel();
            if (crouchCamHandle.IsActive()) crouchCamHandle.Cancel();
        }
    }
}