using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Damage;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data
{
    public enum ProjectileType { Straight, HitScan, Missile, Arc }

    public abstract class ProjectileData : ScriptableObject
    {
        [Serializable]
        public struct ProjectileGeneralSettings
        {
            [SerializeField] ProjectileType projectileType;
            [SerializeField] float damage;
            [SerializeField] DamageType damageType;
            [SerializeField] float maxLiveDuration;

            public readonly ProjectileType ProjectileType => projectileType;
            public readonly float Damage => damage;
            public readonly float MaxLiveDuration => maxLiveDuration;
            public readonly DamageType DamageType => damageType;
        }

        [SerializeField] ProjectileGeneralSettings generalSettings = new();

        public ProjectileGeneralSettings GeneralSettings => generalSettings;
    }
}
