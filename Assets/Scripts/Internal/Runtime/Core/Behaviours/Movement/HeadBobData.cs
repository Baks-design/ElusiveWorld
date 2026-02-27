using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/HeadBobData", order = 3)]
    public class HeadBobData : ScriptableObject
    {
        [Header("Curves")] 
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        [Header("Amplitude")]
        public float xAmplitude;
        public float yAmplitude;
        [Header("Frequency")]
        public float xFrequency;
        public float yFrequency;
        [Header("Run Multipliers")]
        public float runAmplitudeMultiplier;
        public float runFrequencyMultiplier;
        [Header("Crouch Multipliers")]
        public float crouchAmplitudeMultiplier;
        public float crouchFrequencyMultiplier;
      
        public float MoveBackwardsFrequencyMultiplier { get; set; }
        public float MoveSideFrequencyMultiplier { get; set; }
    }
}
