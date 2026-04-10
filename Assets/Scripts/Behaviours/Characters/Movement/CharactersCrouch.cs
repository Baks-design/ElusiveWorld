using UnityEngine;
using System.Collections;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

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

        public void HandleCrouchInput(MonoBehaviour mono, InputManager input)
        {
            if (!checks.CanCrouch()) return;

            if (ShouldSlide(input))
                StartSlide(mono);
            else
                ToggleCrouch(mono);
        }

        public void OnCrouchReleased(MonoBehaviour mono)
        {
            if (flags.isSliding) ReturnToInitHeight(mono);
        }

        bool ShouldSlide(InputManager input)
            => flags.isRunning
            && !flags.isCrouching
            && input.MovementAxis != Vector2.zero
            && checks.CanRun();

        void ToggleCrouch(MonoBehaviour mono)
        {
            if (flags.isCrouching && checks.CheckIfRoof()) return;

            if (effects.landRoutine != null) mono.StopCoroutine(effects.landRoutine);
            if (crouchRoutine != null) mono.StopCoroutine(crouchRoutine);

            crouchRoutine = mono.StartCoroutine(CrouchRoutine(mono));
        }

        IEnumerator CrouchRoutine(MonoBehaviour mono)
        {
            flags.duringCrouchAnimation = true;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            var targetHeight = flags.isCrouching ? flags.initHeight : flags.crouchHeight;
            var targetCenter = flags.isCrouching ? flags.initCenter : flags.crouchCenter;
            var targetCam = flags.isCrouching ? flags.initCamHeight : flags.crouchCamHeight;

            flags.isCrouching = !flags.isCrouching;
            headBob.CurrentStateHeight = targetCam;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, targetHeight, startCenter, targetCenter,
                camStart, targetCam,
                settings.crouchTransitionDuration, settings.crouchTransitionCurve
            ));

            flags.duringCrouchAnimation = false;
        }

        void StartSlide(MonoBehaviour mono)
        {
            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);
            slideRoutine = mono.StartCoroutine(SlideRoutine(mono));
        }

        IEnumerator SlideRoutine(MonoBehaviour mono)
        {
            flags.isSliding = true;
            flags.duringSlideAnimation = true;

            headBob.CurrentStateHeight = flags.slideCamHeight;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, flags.slideHeight, startCenter, flags.slideCenter,
                camStart, flags.slideCamHeight,
                settings.slideTransitionDuration, settings.slideTransitionCurve
            ));

            yield return new WaitForSeconds(settings.maxSlideDuration);

            ReturnToInitHeight(mono);
        }

        public void ReturnToInitHeight(MonoBehaviour mono)
        {
            if (checks.CheckIfRoof())
            {
                flags.isSliding = false;
                ToggleCrouch(mono);
                return;
            }

            if (!flags.isSliding) return;

            if (slideRoutine != null) mono.StopCoroutine(slideRoutine);

            mono.StartCoroutine(ReturnRoutine(mono));
        }

        IEnumerator ReturnRoutine(MonoBehaviour mono)
        {
            flags.isSliding = false;
            flags.duringSlideAnimation = false;

            headBob.CurrentStateHeight = flags.initCamHeight;

            var startHeight = controller.height;
            var startCenter = controller.center;
            var camStart = camPivot.localPosition.y;

            yield return mono.StartCoroutine(LerpAll(
                startHeight, flags.initHeight, startCenter, flags.initCenter,
                camStart, flags.initCamHeight,
                settings.slideTransitionDuration, settings.slideTransitionCurve
            ));
        }

        IEnumerator LerpAll(
           float hStart, float hEnd, Vector3 cStart, Vector3 cEnd,
           float camStart, float camEnd, float duration, AnimationCurve curve)
        {
            var time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                var t = time / duration;
                var eval = curve.Evaluate(t);

                controller.height = Mathf.Lerp(hStart, hEnd, eval);
                controller.center = Vector3.Lerp(cStart, cEnd, eval);

                var pos = camPivot.localPosition;
                pos.y = Mathf.Lerp(camStart, camEnd, eval);
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
