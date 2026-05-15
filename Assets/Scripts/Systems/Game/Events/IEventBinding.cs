using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events
{
    public interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}