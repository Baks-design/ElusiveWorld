using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System.Collections;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCameraEffects
    {
        readonly MonoBehaviour mono;
        readonly MovementSettings settings;
        readonly CharactersFlags flags;
        readonly CharactersChecks checks;
        readonly CharactersLook look;
        readonly Transform camPivot;
        readonly HeadBob headBob;
        readonly InputManager input;

        public CharactersCameraEffects(
            MonoBehaviour mono,
            CharactersFlags flags,
            MovementSettings settings,
            CharactersChecks checks,
            CharactersLook look,
            Transform camPivot,
            HeadBob headBob,
            InputManager input)
        {
            this.mono = mono;
            this.settings = settings;
            this.flags = flags;
            this.checks = checks;
            this.look = look;
            this.camPivot = camPivot;
            this.headBob = headBob;
            this.input = input;
        }

        public void OnPlayerSprintPressed()
        {
            flags.isRunning = true;
            TryStartRunFOV();
        }

        public void OnPlayerSprintReleased()
        {
            flags.isRunning = false;
            StopRunFOV();
        }

        public void Update()
        {
            HandleHeadBob();
            HandleRunFOV();
            look.HandleSway(flags.smoothInputVector, input.MovementAxis.x);
            HandleLanding();
        }

        void HandleLanding()
        {
            if (!flags.previouslyGrounded && flags.isGrounded) StartLandingRoutine();
        }

        void StartLandingRoutine()
        {
            if (flags.landRoutine != null) mono.StopCoroutine(flags.landRoutine);

            flags.landRoutine = LandingRoutine();
            mono.StartCoroutine(flags.landRoutine);
        }

        IEnumerator LandingRoutine()
        {
            var percent = 0f;
            var speed = 1f / settings.landDuration;

            var localPos = camPivot.localPosition;
            var startY = localPos.y;

            var landAmount = flags.inAirTimer > settings.landTimer
                ? settings.highLandAmount
                : settings.lowLandAmount;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;

                var offset = settings.landCurve.Evaluate(percent) * landAmount;
                localPos.y = startY + offset;

                camPivot.localPosition = localPos;
                yield return null;
            }
        }

        void HandleHeadBob()
        {
            var isMoving = input.MovementAxis != Vector2.zero;
            var canBob = flags.isGrounded && !flags.hitWall;

            if (isMoving && canBob)
            {
                if (flags.duringCrouchAnimation || flags.isSliding) return;

                headBob.ScrollHeadBob(
                    flags.isRunning && checks.CanRun(),
                    flags.isCrouching,
                    input.MovementAxis,
                    Time.deltaTime);

                camPivot.localPosition = camPivot.localPosition.ExpDecay(
                    (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                    settings.smoothHeadBobSpeed,
                    Time.deltaTime);

                return;
            }

            if (!headBob.IsReset)
                headBob.ResetHeadBob();

            if (!flags.duringCrouchAnimation)
                camPivot.localPosition = camPivot.localPosition.ExpDecay(
                    new Vector3(0f, headBob.CurrentStateHeight, 0f),
                    settings.smoothHeadBobSpeed,
                    Time.deltaTime);
        }

        void HandleRunFOV() //FIXME
        {
            var isMoving = input.MovementAxis != Vector2.zero;
            var canRun = checks.CanRun();
            var blocked = !isMoving || !canRun || flags.hitWall;

            Debug.Log(blocked);

            if (!flags.duringRunAnimation && !blocked && flags.isRunning)
            {
                flags.duringRunAnimation = true;
                look.ChangeRunFOV(false);
                return;
            }

            if (flags.duringRunAnimation && blocked)
            {
                flags.duringRunAnimation = false;
                look.ChangeRunFOV(true);
            }
        }

        void TryStartRunFOV()
        {
            if (input.MovementAxis == Vector2.zero || !checks.CanRun()) return;

            flags.duringRunAnimation = true;
            look.ChangeRunFOV(false);
        }

        void StopRunFOV()
        {
            if (!flags.duringRunAnimation) return;

            flags.duringRunAnimation = false;
            look.ChangeRunFOV(true);
        }
    }
}