using UnityEngine;
using System.Collections;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCrouch
    {
        readonly CharacterController controller;
        readonly Transform camPivot;
        readonly HeadBob headBob;
        readonly MonoBehaviour mono;
        readonly MovementSettings settings;
        readonly CharactersChecks checks;
        readonly CharactersFlags flags;
        readonly InputManager input;
        Coroutine slideRoutine;

        public CharactersCrouch(
            MonoBehaviour mono,
            MovementSettings settings,
            CharactersChecks checks,
            CharacterController controller,
            CharactersFlags flags,
            Transform camPivot,
            HeadBob headBob,
            InputManager input)
        {
            this.mono = mono;
            this.settings = settings;
            this.checks = checks;
            this.controller = controller;
            this.flags = flags;
            this.camPivot = camPivot;
            this.headBob = headBob;
            this.input = input;

            InitializeSettings();
        }

        void InitializeSettings()
        {
            flags.initCenter = controller.center;
            flags.initHeight = controller.height;
            flags.initCamHeight = camPivot.localPosition.y;

            flags.crouchHeight = flags.initHeight * settings.crouchPercent;
            flags.crouchCenter = (flags.crouchHeight * 0.5f + controller.skinWidth) * Vector3.up;
            flags.crouchStandHeightDifference = flags.initHeight - flags.crouchHeight;
            flags.crouchCamHeight = flags.initCamHeight - flags.crouchStandHeightDifference;

            flags.slideHeight = flags.initHeight * settings.slidePercent;
            flags.slideCenter = (flags.slideHeight * 0.5f + controller.skinWidth) * Vector3.up;
            flags.slideStandHeightDifference = flags.initHeight - flags.slideHeight;
            flags.slideCamHeight = flags.initCamHeight - flags.slideStandHeightDifference;
        }

        public void HandleCrouchInput()
        {
            if (!checks.CanCrouch()) return;

            if (input.MovementAxis != Vector2.zero
                && flags.isRunning
                && !flags.isCrouching
                && checks.CanRun())
                StartSlide();
            else
                ToggleCrouch();
        }

        public void OnCrouchReleased()
        {
            if (flags.isSliding) ReturnToInitHeight();
        }

        void ToggleCrouch()
        {
            if (flags.isCrouching && checks.CheckIfRoof()) return;

            if (flags.landRoutine != null) mono.StopCoroutine(flags.landRoutine);
            if (flags.crouchRoutine != null) mono.StopCoroutine(flags.crouchRoutine);

            flags.crouchRoutine = mono.StartCoroutine(CrouchRoutine());
        }

        IEnumerator CrouchRoutine()
        {
            flags.duringCrouchAnimation = true;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            var toCrouch = !flags.isCrouching;

            var targetHeight = toCrouch ? flags.crouchHeight : flags.initHeight;
            var targetCenter = toCrouch ? flags.crouchCenter : flags.initCenter;
            var targetCam = toCrouch ? flags.crouchCamHeight : flags.initCamHeight;

            flags.isCrouching = toCrouch;
            headBob.CurrentStateHeight = targetCam;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, targetHeight,
                startCenter, targetCenter,
                camStart, targetCam,
                settings.crouchTransitionDuration,
                settings.crouchTransitionCurve));

            flags.duringCrouchAnimation = false;
        }

        void StartSlide()
        {
            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);
            slideRoutine = mono.StartCoroutine(SlideRoutine());
        }

        IEnumerator SlideRoutine()
        {
            flags.isSliding = true;
            flags.duringSlideAnimation = true;

            headBob.CurrentStateHeight = flags.slideCamHeight;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, flags.slideHeight,
                startCenter, flags.slideCenter,
                camStart, flags.slideCamHeight,
                settings.slideTransitionDuration,
                settings.slideTransitionCurve));

            yield return settings.maxSlideDuration.Wait();

            ReturnToInitHeight();
        }

        public void ReturnToInitHeight()
        {
            if (checks.CheckIfRoof())
            {
                flags.isSliding = false;
                ToggleCrouch();
                return;
            }

            if (!flags.isSliding) return;

            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);

            mono.StartCoroutine(ReturnRoutine());
        }

        IEnumerator ReturnRoutine()
        {
            flags.isSliding = false;
            flags.duringSlideAnimation = false;

            headBob.CurrentStateHeight = flags.initCamHeight;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, flags.initHeight,
                startCenter, flags.initCenter,
                camStart, flags.initCamHeight,
                settings.slideTransitionDuration,
                settings.slideTransitionCurve));
        }

        IEnumerator LerpAll(
            float hStart, float hEnd,
            Vector3 cStart, Vector3 cEnd,
            float camStart, float camEnd,
            float duration, AnimationCurve curve)
        {
            var time = 0f;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                var t = time / duration;
                var eval = curve.Evaluate(t);

                controller.height = hStart.Eerp(hEnd, eval);
                controller.center = cStart.Eerp(cEnd, eval);

                var pos = camPivot.localPosition;
                pos.y = camStart.Eerp(camEnd, eval);
                camPivot.localPosition = pos;

                yield return null;
            }

            controller.height = hEnd;
            controller.center = cEnd;

            var final = camPivot.localPosition;
            final.y = camEnd;
            camPivot.localPosition = final;
        }
    }
}
