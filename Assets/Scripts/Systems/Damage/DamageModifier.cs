using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    [Serializable]
    public struct DamageModifier
    {
        public DamageType type;
        [Range(-100f, 100f)] public float percentage;

        public readonly float GetMultiplier() => 1f + (percentage / 100f);
    }
}