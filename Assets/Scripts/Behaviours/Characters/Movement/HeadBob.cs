using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class HeadBob
    {
        readonly HeadBobData data;
        readonly float moveBackwardsMultiplier;
        readonly float moveSideMultiplier;
        Vector3 finalOffset;
        float scroll;

        public Vector3 FinalOffset => finalOffset;
        public bool IsReset { get; private set; }
        public float CurrentStateHeight { get; set; }

        public HeadBob(HeadBobData data, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.data = data;
            this.moveBackwardsMultiplier = moveBackwardsMultiplier;
            this.moveSideMultiplier = moveSideMultiplier;

            ResetImmediate();
        }

        public void ScrollHeadBob(bool isRunning, bool isCrouching, Vector2 input, float deltaTime)
        {
            IsReset = false;

            var amplitude = CalculateAmplitudeMultiplier(isRunning, isCrouching);
            var frequency = CalculateFrequencyMultiplier(isRunning, isCrouching);
            var direction = CalculateDirectionMultiplier(input);

            scroll += deltaTime * data.baseFrequency * frequency;
            scroll = Mathf.Repeat(scroll, 1000f);

            var x = data.xCurve.Evaluate(scroll);
            var y = data.yCurve.Evaluate(scroll);

            var targetOffset = new Vector3(
                x * data.xAmplitude * amplitude * direction,
                y * data.yAmplitude * amplitude * direction,
                0f
            );

            finalOffset = finalOffset.ExpDecay(targetOffset, data.smoothing, deltaTime);
        }

        public void Reset(float deltaTime)
        {
            finalOffset = finalOffset.ExpDecay(Vector3.zero, data.returnSpeed, deltaTime);
            if (finalOffset.sqrMagnitude < 0.0001f)
            {
                finalOffset = Vector3.zero;
                IsReset = true;
            }
        }

        void ResetImmediate()
        {
            scroll = 0f;
            finalOffset = Vector3.zero;
            IsReset = true;
        }

        float CalculateAmplitudeMultiplier(bool isRunning, bool isCrouching)
        {
            var m = 1f;
            if (isRunning) m *= data.runAmplitudeMultiplier;
            if (isCrouching) m *= data.crouchAmplitudeMultiplier;
            return m;
        }

        float CalculateFrequencyMultiplier(bool isRunning, bool isCrouching)
        {
            var m = 1f;
            if (isRunning) m *= data.runFrequencyMultiplier;
            if (isCrouching) m *= data.crouchFrequencyMultiplier;
            return m;
        }

        float CalculateDirectionMultiplier(Vector2 input)
        {
            var backward = Mathf.Clamp01(-input.y);
            var sideways = Mathf.Abs(input.x);
            var m = 1f.ExpDecay(moveBackwardsMultiplier, backward, Time.deltaTime);
            m = m.ExpDecay(moveSideMultiplier, sideways, Time.deltaTime);
            return m;
        }

        public void SetBaseHeight(float height) => CurrentStateHeight = height;
    }
}