namespace VHS
{
    public class StraightProjectile : Projectile
    {
        ProjectileStraightData projectileStraightData;

        protected override void CastData() => projectileStraightData = projectileData as ProjectileStraightData;

        protected override void UpdatePosition(float deltaTime)
        {
            base.UpdatePosition(deltaTime);
            transform.position += deltaTime * projectileStraightData.SpecificSettings.Speed * currentDirection;
        }
    }
}
