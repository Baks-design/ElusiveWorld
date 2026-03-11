using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileStraightData", menuName = "Data/Behaviours/Projectiles/Straight")]
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