using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Tendency.Interfaces
{
    public interface ITendencySystem
    {
        float GlobalTendency { get; }

        event Action<TendencyState> OnTendencyChanged;

        void AddTendency(float amount);
        void RegisterAction(string actionId, float impact);
    }
}