namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events
{
    public struct InteractEvent : IEvent
    {
        public bool resetUI;
        public string setTooltip;
        public float updateProgress;
    }
}