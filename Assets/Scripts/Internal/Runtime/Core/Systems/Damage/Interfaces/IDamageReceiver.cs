namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IDamageReceiver
    {
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}