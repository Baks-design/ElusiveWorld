using Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Base;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Types
{
    public class PlayerDamageReceiver : DamageReceiver
    {
        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            Debug.Log($"Player took {amount} damage. Health: {currentHealth}");
        }

        protected override void Die() => Debug.Log("Player died");
    }
}