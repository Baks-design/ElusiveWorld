using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersCameraEffects //FIXME: TASK
    {
        readonly MovementSettings settings;
        readonly CharactersFlags flags;
        readonly CharactersChecks checks;
        readonly CharactersLook look;
        readonly Transform camPivot;
        readonly HeadBob headBob;
        public CancellationTokenSource landingTokenSource;

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

        public void OnPlayerSprintPressed()
        {
            flags.isRunning = true;
            ChangeToRunFOV();
        }

        public void OnPlayerSprintReleased()
        {
            flags.isRunning = false;
            ChangeToInitFOV();
        }

        public void Update(InputManager input)
        {
            HandleHeadBob(input);
            HandleRunFOV(input);
            HandleCameraSway(input);
            HandleLanding();
        }

        void HandleLanding()
        {
            if (!flags.previouslyGrounded && flags.isGrounded) InvokeLandingRoutine();
        }

        void InvokeLandingRoutine()
        {
            landingTokenSource?.Cancel();
            landingTokenSource?.Dispose();
            landingTokenSource = new();
            LandingRoutine(landingTokenSource.Token).Forget();
        }

        async UniTask LandingRoutine(CancellationToken source)
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

                await UniTask.Yield(PlayerLoopTiming.Update, source);
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

        void ChangeToRunFOV()
        {
            if (!checks.CanRun()
                || flags.finalMoveVector.sqrMagnitude < 0.1f) return;

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