using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileMissileData", menuName = "Data/Behaviours/Projectiles/Missile")]
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