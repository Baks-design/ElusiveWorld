using System;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
{
    [Serializable]
    public class TendencyThreshold
    {
        public TendencyState state;
        public float minValue;
        public float maxValue;
    }
}