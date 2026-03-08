using System;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
{
    [Serializable]
    public class ActionImpact
    {
        public string actionId;
        public float impact;
        public bool isRegional;
    }
}