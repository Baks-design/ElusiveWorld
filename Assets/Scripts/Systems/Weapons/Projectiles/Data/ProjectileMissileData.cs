using System;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileMissileData", menuName = "Data/Systems/Weapons/Projectiles/Missile")]
    public class ProjectileMissileData : ProjectileData
    {
        [Serializable]
        public struct ProjectileMissileSettings
        {
            [SerializeField] float followDuration;

            public readonly float FollowDuration => followDuration;
        }
        
        [SerializeField] ProjectileMissileSettings specificSettings = new();
        
        public ProjectileMissileSettings SpecificSettings => specificSettings;
    }
}