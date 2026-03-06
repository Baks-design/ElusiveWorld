using Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Base
{
    public abstract class DamageReceiver : MonoBehaviour, IDamageReceiver //TODO: Add other Bases
    {
        [SerializeField] protected float maxHealth = 100f;
        protected float currentHealth;

        public bool IsAlive => currentHealth > 0;

        protected virtual void Start() => currentHealth = maxHealth;

        public virtual void TakeDamage(float amount)
        {
            if (!IsAlive) return;

            currentHealth -= amount;
            if (currentHealth <= 0f)
                Die();
        }

        protected abstract void Die();
    }
}