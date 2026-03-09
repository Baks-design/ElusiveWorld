namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Interfaces
{
    public interface IDamageReceiver
    {
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}