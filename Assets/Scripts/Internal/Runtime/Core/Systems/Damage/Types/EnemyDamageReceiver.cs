using Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Base;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Types
{
    public class EnemyDamageReceiver : DamageReceiver
    {
        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            Debug.Log($"Enemy took {amount} damage. Health: {currentHealth}");
        }

        protected override void Die() => Destroy(gameObject);
    }
}