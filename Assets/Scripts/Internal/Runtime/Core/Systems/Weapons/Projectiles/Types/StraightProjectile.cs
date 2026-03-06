using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Types
{
    public class StraightProjectile : Projectile //TODO: Apply Damage
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