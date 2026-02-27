using System;
using UnityEngine;

namespace VHS
{
    [CreateAssetMenu( menuName = "Systems/Weapon/Projectiles/HitScan")]
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