namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IDamageDealer
    {
        float DamageAmount { get; }

        void DealDamage(IDamageReceiver target);
    }
}