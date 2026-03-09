using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles.Base;
using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles.Data;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles.Types
{
    public class StraightProjectile : Projectile 
    {
        ProjectileStraightData projectileStraightData;

        protected override void CastData() => projectileStraightData = projectileData as ProjectileStraightData;

        protected override void UpdatePosition(float deltaTime)
        {
            transform.position += deltaTime * projectileStraightData.SpecificSettings.Speed * currentDirection;
            base.UpdatePosition(deltaTime);
        }
    }
}