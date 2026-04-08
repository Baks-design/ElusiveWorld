using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class HeadBob
    {
        readonly HeadBobData data;
        readonly float moveBackwardsMultiplier;
        readonly float moveSideMultiplier;
        Vector3 finalOffset;
        float xScroll;
        float yScroll;

        public Vector3 FinalOffset => finalOffset;
        public bool IsReset { get; private set; }
        public float CurrentStateHeight { get; set; } = 0f;

        public HeadBob(HeadBobData data, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.data = data;
            this.moveBackwardsMultiplier = moveBackwardsMultiplier;
            this.moveSideMultiplier = moveSideMultiplier;

            ResetHeadBob();
        }

        public void ScrollHeadBob(bool isRunning, bool isCrouching, Vector2 input, float deltaTime)
        {
            IsReset = false;

            var amplitudeMultiplier = CalculateAmplitudeMultiplier(isRunning, isCrouching);
            var frequencyMultiplier = CalculateFrequencyMultiplier(isRunning, isCrouching);
            var directionMultiplier = CalculateDirectionMultiplier(input);

            xScroll += deltaTime * data.xFrequency * frequencyMultiplier;
            yScroll += deltaTime * data.yFrequency * frequencyMultiplier;

            var xValue = data.xCurve.Evaluate(xScroll);
            var yValue = data.yCurve.Evaluate(yScroll);

            finalOffset.x = xValue * data.xAmplitude * amplitudeMultiplier * directionMultiplier;
            finalOffset.y = yValue * data.yAmplitude * amplitudeMultiplier * directionMultiplier;
        }

        public void ResetHeadBob()
        {
            IsReset = true;
            xScroll = 0f;
            yScroll = 0f;
            finalOffset = Vector3.zero;
        }

        float CalculateAmplitudeMultiplier(bool isRunning, bool isCrouching)
        {
            var multiplier = 1f;

            if (isRunning) multiplier *= data.runAmplitudeMultiplier;
            if (isCrouching) multiplier *= data.crouchAmplitudeMultiplier;
            
            return multiplier;
        }

        float CalculateFrequencyMultiplier(bool isRunning, bool isCrouching)
        {
            var multiplier = 1f;

            if (isRunning) multiplier *= data.runFrequencyMultiplier;
            if (isCrouching) multiplier *= data.crouchFrequencyMultiplier;

            return multiplier;
        }

        float CalculateDirectionMultiplier(Vector2 input)
        {
            var isMovingBackwards = input.y < -0.1f;
            if (isMovingBackwards) return moveBackwardsMultiplier;

            var isMovingSideways = Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f;
            if (isMovingSideways) return moveSideMultiplier;

            return 1f;
        }
    }
}