using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System.Collections;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCameraEffects
    {
        readonly MovementSettings settings;
        readonly CharactersFlags flags;
        readonly CharactersChecks checks;
        readonly CharactersLook look;
        readonly Transform camPivot;
        readonly HeadBob headBob;
        public IEnumerator landRoutine;

        public CharactersCameraEffects(
            CharactersFlags flags,
            MovementSettings settings,
            CharactersChecks checks,
            CharactersLook look,
            Transform camPivot,
            HeadBob headBob)
        {
            this.settings = settings;
            this.flags = flags;
            this.checks = checks;
            this.look = look;
            this.camPivot = camPivot;
            this.headBob = headBob;
        }

        public void OnPlayerSprintPressed(InputManager input) //FIXME
        {
            flags.isRunning = true;
            ChangeToRunFOV(input);
        }

        public void OnPlayerSprintReleased()
        {
            flags.isRunning = false;
            ChangeToInitFOV();
        }

        public void Update(MonoBehaviour mono, InputManager input)
        {
            HandleHeadBob(input);
            HandleRunFOV(input);
            HandleCameraSway(input);
            HandleLanding(mono);
        }

        void HandleLanding(MonoBehaviour mono)
        {
            if (!flags.previouslyGrounded && flags.isGrounded) InvokeLandingRoutine(mono);
        }

        void InvokeLandingRoutine(MonoBehaviour mono)
        {
            if (landRoutine != null) mono.StopCoroutine(landRoutine);
            landRoutine = LandingRoutine();
            mono.StartCoroutine(landRoutine);
        }

        IEnumerator LandingRoutine()
        {
            var percent = 0f;
            var speed = 1f / settings.landDuration;
            var localPos = camPivot.localPosition;
            var initLandHeight = localPos.y;
            var landAmount = flags.inAirTimer > settings.landTimer
                ? settings.highLandAmount : settings.lowLandAmount;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var desiredY = settings.landCurve.Evaluate(percent) * landAmount;
                localPos.y = initLandHeight + desiredY;
                camPivot.localPosition = localPos;
                yield return null;
            }
        }

        public void HandleHeadBob(InputManager input)
        {
            if (input.MovementAxis != Vector2.zero && flags.isGrounded && !flags.hitWall)
            {
                if (!flags.duringCrouchAnimation && !flags.isSliding)
                {
                    headBob.ScrollHeadBob(
                        flags.isRunning && checks.CanRun(),
                        flags.isCrouching,
                        input.MovementAxis,
                        Time.deltaTime);

                    camPivot.localPosition = camPivot.localPosition.ExpDecay(
                        (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                        settings.smoothHeadBobSpeed,
                        Time.deltaTime);
                }
            }
            else
            {
                if (!headBob.IsReset)
                    headBob.ResetHeadBob();

                if (!flags.duringCrouchAnimation)
                    camPivot.localPosition = camPivot.localPosition.ExpDecay(
                        new Vector3(0f, headBob.CurrentStateHeight, 0f),
                        settings.smoothHeadBobSpeed,
                        Time.deltaTime);
            }
        }

        void HandleCameraSway(InputManager input) =>
            look.HandleSway(flags.smoothInputVector, input.MovementAxis.x);

        void HandleRunFOV(InputManager input)
        {
            if (!flags.duringRunAnimation
                && input.MovementAxis != Vector2.zero
                && !flags.hitWall && flags.isRunning && checks.CanRun())
            {
                flags.duringRunAnimation = true;
                look.ChangeRunFOV(false);
            }

            if (flags.duringRunAnimation
                && (input.MovementAxis == Vector2.zero
                || !checks.CanRun()
                || flags.hitWall))
            {
                flags.duringRunAnimation = false;
                look.ChangeRunFOV(true);
            }
        }

        void ChangeToRunFOV(InputManager input)
        {
            if (!checks.CanRun() || input.MovementAxis == Vector2.zero) return;
            flags.duringRunAnimation = true;
            look.ChangeRunFOV(false);
        }

        void ChangeToInitFOV()
        {
            if (!flags.duringRunAnimation) return;
            flags.duringRunAnimation = false;
            look.ChangeRunFOV(true);
        }
    }
}