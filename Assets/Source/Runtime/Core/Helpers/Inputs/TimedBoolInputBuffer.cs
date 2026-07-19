using System;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    [Serializable]
    public class TimedBoolInputBuffer : TimedInputBuffer<bool>
    {
        public void Set() => Set(true);

        public bool TryConsume() => TryConsume(out _);
    }
}