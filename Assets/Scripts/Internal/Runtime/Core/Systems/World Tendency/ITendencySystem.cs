using System;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
{
    public interface ITendencySystem
    {
        float GlobalTendency { get; }

        event Action<TendencyState> OnTendencyChanged;

        void AddTendency(float amount);
        void RegisterAction(string actionId, float impact);
    }
}