namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
{
    public interface ITendencyEffect
    {
        TendencyState RequiredState { get; }

        void Apply();
        void Remove();
    }
}