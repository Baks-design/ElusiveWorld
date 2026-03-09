namespace ElusiveWorld.Core.Assets.Scripts.Systems.Tendency.Interfaces
{
    public interface ITendencyEffect
    {
        TendencyState RequiredState { get; }

        void Apply();
        void Remove();
    }
}