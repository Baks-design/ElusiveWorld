namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IResurrectable
    {
        int LivesRemaining { get; }

        void Resurrect();
    }
}