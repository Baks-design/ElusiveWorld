using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement.Data;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class HeadBob
    {
        readonly HeadBobData data;
        Vector3 finalOffset;
        float xScroll;
        float yScroll;
        float xValue;
        float yValue;
        float amplitudeMultiplier;
        float frequencyMultiplier;
        float additionalMultiplier;

        public Vector3 FinalOffset => finalOffset;
        public bool Resetted { get; private set; }
        public float CurrentStateHeight { get; set; } = 0f;

        public HeadBob(HeadBobData data, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.data = data;

            data.MoveBackwardsFrequencyMultiplier = moveBackwardsMultiplier;
            data.MoveSideFrequencyMultiplier = moveSideMultiplier;
            xScroll = 0f;
            yScroll = 0f;
            Resetted = false;
            finalOffset = Vector3.zero;
        }

        public void ScrollHeadBob(bool running, bool crouching, Vector2 input)
        {
            Resetted = false;

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
            Resetted = true;
            xScroll = 0f;
            yScroll = 0f;
            finalOffset = Vector3.zero;
        }
    }
}
