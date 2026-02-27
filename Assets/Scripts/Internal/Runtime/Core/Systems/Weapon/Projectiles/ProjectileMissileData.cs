using System;
using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Systems/WeaponProjectiles/Missile")]
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