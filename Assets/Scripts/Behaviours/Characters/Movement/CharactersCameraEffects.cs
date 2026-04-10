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

        public void OnPlayerSprintPressed(InputManager input)
        {
            flags.isRunning = true;
            TryStartRunFOV(input);
        }

        public void OnPlayerSprintReleased()
        {
            flags.isRunning = false;
            StopRunFOV();
        }

        public void Update(MonoBehaviour mono, InputManager input, float dt)
        {
            HandleHeadBob(input, dt);
            HandleRunFOV(input);
            look.HandleSway(flags.smoothInputVector, input.MovementAxis.x);
            HandleLanding(mono, dt);
        }

        void HandleLanding(MonoBehaviour mono, float dt)
        {
            if (!flags.previouslyGrounded && flags.isGrounded) StartLandingRoutine(mono, dt);
        }

        void StartLandingRoutine(MonoBehaviour mono, float dt)
        {
            if (landRoutine != null) mono.StopCoroutine(landRoutine);

            landRoutine = LandingRoutine(dt);
            mono.StartCoroutine(landRoutine);
        }

        IEnumerator LandingRoutine(float dt)
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
                percent += dt * speed;

                var offset = settings.landCurve.Evaluate(percent) * landAmount;
                localPos.y = startY + offset;

                camPivot.localPosition = localPos;
                yield return null;
            }
        }

        void HandleHeadBob(InputManager input, float dt)
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
                    dt);

                camPivot.localPosition = camPivot.localPosition.ExpDecay(
                    (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset,
                    settings.smoothHeadBobSpeed,
                    dt);

                return;
            }

            if (!headBob.IsReset)
                headBob.ResetHeadBob();

            if (!flags.duringCrouchAnimation)
                camPivot.localPosition = camPivot.localPosition.ExpDecay(
                    new Vector3(0f, headBob.CurrentStateHeight, 0f),
                    settings.smoothHeadBobSpeed,
                    dt);
        }

        void HandleRunFOV(InputManager input)
        {
            var isMoving = input.MovementAxis != Vector2.zero;
            var canRun = checks.CanRun();
            var blocked = !isMoving || !canRun || flags.hitWall;

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

        void TryStartRunFOV(InputManager input)
        {
            if (!checks.CanRun() || input.MovementAxis == Vector2.zero) return;

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