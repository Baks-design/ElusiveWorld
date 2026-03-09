using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Tendency
{
    [Serializable]
    public class TendencyThreshold
    {
        public TendencyState state;
        public float minValue;
        public float maxValue;
    }
}