using System;
using UnityEngine;

namespace VHS
{
    public enum ProjectileType { Straight, HitScan, Missile, Arc }

    public abstract class ProjectileData : ScriptableObject
    {
        [Serializable]
        public struct ProjectileGeneralSettings
        {
            [SerializeField] ProjectileType projectileType;
            [SerializeField] float damage;
            [SerializeField] float maxLiveDuration;

            public readonly ProjectileType ProjectileType => projectileType;
            public readonly float Damage => damage;
            public readonly float MaxLiveDuration => maxLiveDuration;
        }

        [Header("Projectile Data")]
        [SerializeField] ProjectileGeneralSettings generalSettings = new();

        public ProjectileGeneralSettings GeneralSettings => generalSettings;
    }
}
