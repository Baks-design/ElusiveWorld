namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Interfaces
{
    public interface IResurrectable
    {
        int LivesRemaining { get; }
        bool CanResurrect { get; }
        
        void Resurrect();
    }
}