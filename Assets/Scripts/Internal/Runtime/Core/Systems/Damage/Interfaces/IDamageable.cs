namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IDamageable
    {
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}