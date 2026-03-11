using ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Base;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Types
{
    public class HitScanProjectile : Projectile
    {
        ProjectileHitScanData projectileHitScanData;
        protected override void CastData() 
            => projectileHitScanData = projectileData as ProjectileHitScanData;

        public override void OnFire(Transform owner)
        {
            base.OnFire(owner);
            CheckForCollision();
        }

        public override void CheckForCollision()
        {
            if (Physics.Linecast(
                transform.position,
                transform.position + transform.forward * projectileHitScanData.SpecificSettings.ScanDistance,
                out hitInfo,
                collisionLayers))
            {
                if (hitInfo.collider.transform.IsChildOf(transform) ||
                    hitInfo.collider.transform.IsChildOf(owner))
                    return;

                OnHit();
            }
        }

        public override void OnHit() => base.OnHit();
    }
}