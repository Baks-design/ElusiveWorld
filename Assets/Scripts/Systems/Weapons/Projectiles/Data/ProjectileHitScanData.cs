using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileHitScanData", menuName = "Data/Systems/Weapons/Projectiles/HitScan")]
    public class ProjectileHitScanData : ProjectileData
    {
        [Serializable]
        public struct ProjectileHitScanSettings
        {
            [SerializeField] float scanDistance;

            public readonly float ScanDistance => scanDistance;
        }

        [SerializeField] ProjectileHitScanSettings specificSettings = new();

        public ProjectileHitScanSettings SpecificSettings => specificSettings;
    }
}