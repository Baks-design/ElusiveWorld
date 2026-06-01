namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events
{
    public struct LoadingScreenEvent : IEvent
    {
        public float currentProgress;
        public float targetProgress;
        public bool enableCanvas;
    }
}