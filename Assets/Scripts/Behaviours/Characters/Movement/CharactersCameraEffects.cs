using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

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
        readonly InputManager input;
        float landingTime;
        float landingDuration;
        float landingAmount;
        bool isLanding;

        public CharactersCameraEffects(
            MovementSettings settings,
            CharactersFlags flags,
            CharactersChecks checks,
            CharactersLook look,
            Transform camPivot,
            HeadBob headBob,
            InputManager input)
        {
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
            if (!checks.CanRun() || input.MovementAxis == Vector2.zero) return;

            flags.isRunning = true;
            look.ChangeRunFOV(false);
        }

        public void OnPlayerSprintReleased()
        {
            flags.isRunning = false;
            look.ChangeRunFOV(true);
        }

        public void Update()
        {
            HandleLandingTrigger();
            UpdateLanding();

            var headbobOffset = HandleHeadBob();
            var landingOffset = GetLandingOffset();
            ApplyFinalPosition(headbobOffset + landingOffset);

            look.HandleSway(input.MovementAxis.x);
        }

        void HandleLandingTrigger()
        {
            if (flags.previouslyGrounded || !flags.isGrounded || isLanding) return;
            
            isLanding = true;
            landingTime = 0f;

            landingDuration = Mathf.Max(0.0001f, settings.landDuration);

            landingAmount = flags.inAirTimer > settings.landTimer
                ? settings.highLandAmount
                : settings.lowLandAmount;
        }

        void UpdateLanding()
        {
            if (!isLanding) return;

            landingTime += Time.deltaTime;
            if (landingTime >= landingDuration)
                isLanding = false;
        }

        Vector3 GetLandingOffset()
        {
            if (!isLanding) return Vector3.zero;

            var percent = landingTime / landingDuration;
            var offset = settings.landCurve.Evaluate(percent) * landingAmount;
            return new Vector3(0f, offset, 0f);
        }

        Vector3 HandleHeadBob()
        {
            var isMoving = input.MovementAxis != Vector2.zero;
            var canBob = flags.isGrounded && !flags.hitWall;

            if (isMoving && canBob)
            {
                if (flags.duringCrouchAnimation || flags.isSliding) return Vector3.zero;

                headBob.ScrollHeadBob(
                    flags.isRunning && checks.CanRun(),
                    flags.isCrouching,
                    input.MovementAxis,
                    Time.deltaTime);

                return (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset;
            }

            if (!headBob.IsReset)
                headBob.Reset(Time.deltaTime);

            return new Vector3(0f, headBob.CurrentStateHeight, 0f);
        }

        void ApplyFinalPosition(Vector3 targetOffset) =>
            camPivot.localPosition = camPivot.localPosition.ExpDecay(
                targetOffset,
                settings.smoothHeadBobSpeed,
                Time.deltaTime);
    }
}