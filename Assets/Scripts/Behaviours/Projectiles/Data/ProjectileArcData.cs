using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data
{
    [CreateAssetMenu(fileName = "ProjectileArcData", menuName = "Data/Behaviours/Projectiles/Arc")]
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