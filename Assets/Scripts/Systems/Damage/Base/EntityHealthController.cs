using Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Components;
using Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Base
{
    [RequireComponent(typeof(Health))]
    public abstract class EntityHealthController : 
        MonoBehaviour, IDamageReceiver, IHealable, IResurrectable
    {
        [SerializeField] protected Health health;

        public bool IsAlive => health != null && health.IsAlive;
        public float HealthPercentage => health != null ? health.CurrentHealth / health.MaxHealth : 0f;
        public int LivesRemaining => health != null ? health.CurrentLives : 0;
        public bool CanResurrect => health != null && !IsAlive && health.CurrentLives > 0;

        protected virtual void Awake()
        {
            if (health == null) health = GetComponent<Health>();
        }

        protected virtual void Start() { }

        public virtual void TakeDamage(float amount)
        {
            if (amount <= 0f || !IsAlive || health == null) return;

            var previousHealth = health.CurrentHealth;
            health.ModifyHealth(-amount);

            var actualDamage = previousHealth - health.CurrentHealth;
            OnDamageTaken(actualDamage);

            if (!IsAlive) Die();
        }

        public virtual void Heal(float amount)
        {
            if (amount <= 0f || !IsAlive || health == null) return;

            var previousHealth = health.CurrentHealth;
            health.ModifyHealth(amount);

            var actualHeal = health.CurrentHealth - previousHealth;
            OnHealed(actualHeal);
        }

        public virtual void Resurrect()
        {
            if (!CanResurrect || health == null) return;

            health.DecreaseLife();
            health.ResetHealth();
            OnResurrected();
        }

        protected virtual void OnDamageTaken(float amount) =>
            Debug.Log($"{gameObject.name} took {amount} damage. Health: {health.CurrentHealth}/{health.MaxHealth}");

        protected virtual void OnHealed(float amount) =>
            Debug.Log($"{gameObject.name} healed {amount}. Health: {health.CurrentHealth}/{health.MaxHealth}");

        protected virtual void OnResurrected() =>
            Debug.Log($"{gameObject.name} resurrected. Lives remaining: {health.CurrentLives}");

        protected abstract void Die();
    }
}