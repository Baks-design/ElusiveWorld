using System;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileStraightData", menuName = "Data/Systems/Weapons/Projectiles/Straight")]
    public class ProjectileStraightData : ProjectileData
    {
        [Serializable]
        public struct ProjectileStraightSettings
        {
            [SerializeField] float speed;

            public readonly float Speed => speed;
        }

        [SerializeField] protected ProjectileStraightSettings specificSettings = new();

        public ProjectileStraightSettings SpecificSettings => specificSettings;
    }
}