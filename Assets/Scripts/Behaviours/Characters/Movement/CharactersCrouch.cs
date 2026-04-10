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
        readonly MovementSettings settings;
        readonly CharactersChecks checks;
        readonly CharactersFlags flags;
        readonly CharactersCameraEffects effects;
        Coroutine slideRoutine;
        public Coroutine crouchRoutine;

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
            this.camPivot = camPivot;
            this.headBob = headBob;
            this.effects = effects;

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

        public void HandleCrouchInput(MonoBehaviour mono, InputManager input, float dt)
        {
            if (!checks.CanCrouch()) return;

            if (flags.isRunning && !flags.isCrouching && input.MovementAxis != Vector2.zero && checks.CanRun())
                StartSlide(mono, dt);
            else
                ToggleCrouch(mono, dt);
        }

        public void OnCrouchReleased(MonoBehaviour mono, float dt)
        {
            if (flags.isSliding) ReturnToInitHeight(mono, dt);
        }

        void ToggleCrouch(MonoBehaviour mono, float dt)
        {
            if (flags.isCrouching && checks.CheckIfRoof()) return;

            if (effects.landRoutine != null) mono.StopCoroutine(effects.landRoutine);
            if (crouchRoutine != null) mono.StopCoroutine(crouchRoutine);

            crouchRoutine = mono.StartCoroutine(CrouchRoutine(mono, dt));
        }

        IEnumerator CrouchRoutine(MonoBehaviour mono, float dt)
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
                settings.crouchTransitionCurve,
                dt));

            flags.duringCrouchAnimation = false;
        }

        void StartSlide(MonoBehaviour mono, float dt)
        {
            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);
            slideRoutine = mono.StartCoroutine(SlideRoutine(mono, dt));
        }

        IEnumerator SlideRoutine(MonoBehaviour mono, float dt)
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
                settings.slideTransitionCurve,
                dt));

            yield return IEnumeratorExtensions.Wait(settings.maxSlideDuration);

            ReturnToInitHeight(mono, dt);
        }

        public void ReturnToInitHeight(MonoBehaviour mono, float dt)
        {
            if (checks.CheckIfRoof())
            {
                flags.isSliding = false;
                ToggleCrouch(mono, dt);
                return;
            }

            if (!flags.isSliding) return;

            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);

            mono.StartCoroutine(ReturnRoutine(mono, dt));
        }

        IEnumerator ReturnRoutine(MonoBehaviour mono, float dt)
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
                settings.slideTransitionCurve,
                dt));
        }

        IEnumerator LerpAll(
            float hStart, float hEnd,
            Vector3 cStart, Vector3 cEnd,
            float camStart, float camEnd,
            float duration, AnimationCurve curve, float dt)
        {
            var time = 0f;

            while (time < duration)
            {
                time += dt;
                var t = time / duration;
                var eval = curve.Evaluate(t);

                controller.height = hStart.Eerp(hEnd, eval);
                controller.center = cStart.Eerp(cEnd, eval);

                var pos = camPivot.localPosition;
                pos.y = camStart.ExpDecay(camEnd, eval, eval);
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
