using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    [Serializable]
    public class CharacterStats
    {
        [Header("Base Stats")]
        public float maxHealth = 100f;
        [Header("Defense")]
        public float armor = 0f;
        public float dodgeChance = 0f;
        public float blockChance = 0f;
        public float blockReduction = 0.5f;
        [Header("Damage Modifiers")]
        public List<DamageModifier> damageDealtModifiers = new();
        public List<DamageResistance> damageResistances = new();
        [Header("Critical")]
        public float criticalChance = 0.05f;
        public float criticalDamage = 1.5f;

        [field: SerializeField] public float CurrentHealth { get; private set; }

        public void Initialize() => CurrentHealth = maxHealth;

        public float GetDamageModifier(DamageType type)
        {
            var modifier = 1f;
            foreach (var mod in damageDealtModifiers)
                if (mod.type == type)
                    modifier *= mod.GetMultiplier();
            return modifier;
        }

        public float GetResistance(DamageType type)
        {
            var resistance = 0f;
            foreach (var res in damageResistances)
            {
                if (res.type == type)
                {
                    resistance = res.reduction;
                    break;
                }
            }
            return resistance;
        }
    }
}