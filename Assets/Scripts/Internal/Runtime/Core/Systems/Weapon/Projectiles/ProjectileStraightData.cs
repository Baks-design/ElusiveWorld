using System;
using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Systems/Weapon/Projectiles/Straight")]
    public class ProjectileStraightData : ProjectileData
    {
        [Serializable]
        public struct ProjectileStraightSettings
        {
            [SerializeField] private float speed;

            public readonly float Speed => speed;
        }

        [SerializeField] protected ProjectileStraightSettings specificSettings = new();

        public ProjectileStraightSettings SpecificSettings => specificSettings;
    }
}