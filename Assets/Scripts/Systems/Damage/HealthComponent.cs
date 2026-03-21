using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] bool initializeOnStart = true;
        [field: SerializeField] CharacterStats stats;

        public bool IsAlive { get; private set; } = true;
        public float CurrentHealth { get; private set; }
        public float MaxHealth => stats?.maxHealth ?? 0f;
        public float HealthPercentage => CurrentHealth / stats.maxHealth;
        bool CheckCritical => UnityEngine.Random.value < stats.criticalChance;
        bool CheckDodge => UnityEngine.Random.value < stats.dodgeChance;
        bool CheckBlock => UnityEngine.Random.value < stats.blockChance;

        public event Action<DamageInfo, float> OnDamageTaken = delegate { };
        public event Action OnDeath = delegate { };

        void Start()
        {
            if (initializeOnStart && stats != null)
            {
                stats.Initialize();
                CurrentHealth = stats.maxHealth;
                IsAlive = true;
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive) return;

            damageInfo.target = gameObject;
            var result = CalculateDamage(damageInfo);
            if (result.isDodged)
            {
                OnDamageTaken.Invoke(damageInfo, 0f);
                return;
            }

            var actualDamage = result.isBlocked ?
                result.finalDamage * (1f - stats.blockReduction) :
                result.finalDamage;

            CurrentHealth -= actualDamage;
            OnDamageTaken.Invoke(damageInfo, actualDamage);

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                Die();
            }
        }

        DamageResult CalculateDamage(DamageInfo damageInfo)
        {
            var result = new DamageResult
            {
                rawDamage = damageInfo.baseDamage,
                damageType = damageInfo.damageType,
                isCritical = damageInfo.isCritical || CheckCritical,
                isDodged = CheckDodge,
                isBlocked = CheckBlock
            };

            if (result.isDodged)
            {
                result.finalDamage = 0f;
                result.mitigation = 1f;
                return result;
            }

            var damage = result.rawDamage;
            damage *= stats.GetDamageModifier(damageInfo.damageType);

            if (result.isCritical)
                damage *= stats.criticalDamage;

            if (!damageInfo.ignoresResistances)
            {
                var resistance = stats.GetResistance(damageInfo.damageType);
                damage *= 1f - resistance;

                if (damageInfo.damageType == DamageType.Physical)
                {
                    var armorReduction = stats.armor / (stats.armor + 100f);
                    damage *= 1f - armorReduction;
                }
            }

            result.finalDamage = Mathf.Max(0, damage);
            result.mitigation = 1f - (result.finalDamage / result.rawDamage);

            return result;
        }

        void Die()
        {
            IsAlive = false;
            OnDeath.Invoke();
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Min(CurrentHealth + amount, stats.maxHealth);
        }

        public void Revive(float healthPercentage = 1f)
        {
            IsAlive = true;
            CurrentHealth = stats.maxHealth * Mathf.Clamp01(healthPercentage);
        }

        public void ResetHealth()
        {
            CurrentHealth = stats.maxHealth;
            IsAlive = true;
        }
    }
}