using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    public class HeadBob
    {
        readonly HeadBobData data;
        Vector3 finalOffset;
        bool resetted;
        float xScroll;
        float yScroll;
        float currentStateHeight = 0f;
        float amplitudeMultiplier;
        float frequencyMultiplier;
        float additionalMultiplier;
        float xValue;
        float yValue;

        public Vector3 FinalOffset => finalOffset;
        public bool Resetted => resetted;
        public float CurrentStateHeight
        {
            get => currentStateHeight;
            set => currentStateHeight = value;
        }

        public HeadBob(HeadBobData data, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.data = data;
            
            data.MoveBackwardsFrequencyMultiplier = moveBackwardsMultiplier;
            data.MoveSideFrequencyMultiplier = moveSideMultiplier;
            xScroll = 0f;
            yScroll = 0f;
            resetted = false;
            finalOffset = Vector3.zero;
        }

        public void ScrollHeadBob(bool running, bool crouching, Vector2 input)
        {
            resetted = false;

            amplitudeMultiplier = running ? data.runAmplitudeMultiplier : 1f;
            amplitudeMultiplier = crouching ? data.crouchAmplitudeMultiplier : amplitudeMultiplier;

            frequencyMultiplier = running ? data.runFrequencyMultiplier : 1f;
            frequencyMultiplier = crouching ? data.crouchFrequencyMultiplier : frequencyMultiplier;

            additionalMultiplier = input.y == -1f ? data.MoveBackwardsFrequencyMultiplier : 1f;
            additionalMultiplier = input.x != 0f & input.y == 0f ? data.MoveSideFrequencyMultiplier : additionalMultiplier;

            xScroll += Time.deltaTime * data.xFrequency * frequencyMultiplier;
            yScroll += Time.deltaTime * data.yFrequency * frequencyMultiplier;

            xValue = data.xCurve.Evaluate(xScroll);
            yValue = data.yCurve.Evaluate(yScroll);

            finalOffset.x = xValue * data.xAmplitude * amplitudeMultiplier * additionalMultiplier;
            finalOffset.y = yValue * data.yAmplitude * amplitudeMultiplier * additionalMultiplier;
        }

        public void ResetHeadBob()
        {
            resetted = true;
            xScroll = 0f;
            yScroll = 0f;
            finalOffset = Vector3.zero;
        }
    }
}