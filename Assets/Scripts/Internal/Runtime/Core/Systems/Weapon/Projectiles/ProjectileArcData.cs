using System;
using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Systems/Weapon/Projectiles/Arc")]
    public class ProjectileArcData : ProjectileData
    {
        [Serializable]
        public struct ProjectileArcSettings
        {
            [SerializeField] float angle;

            public readonly float Angle => angle;
        }

        [SerializeField] ProjectileArcSettings specificSettings = new();

        public ProjectileArcSettings SpecificSettings => specificSettings;
    }
}