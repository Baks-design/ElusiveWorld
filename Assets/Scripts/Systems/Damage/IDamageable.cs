using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    public interface IDamageable
    {
        bool IsAlive { get; }

        event Action<DamageInfo, float> OnDamageTaken;
        event Action OnDeath;

        void TakeDamage(DamageInfo damageInfo);
    }
}