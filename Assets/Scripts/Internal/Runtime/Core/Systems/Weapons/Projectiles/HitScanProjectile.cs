using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles
{
    public class HitScanProjectile : Projectile
    {
        ProjectileHitScanData projectileHitScanData;
         
        public override bool IsPoolable => true;
        protected override void CastData() => projectileHitScanData = projectileData as ProjectileHitScanData;

        protected override void OnEnable() {}
        protected override void OnDisable() {}

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

        public override void OnHit() => Debug.Log(hitInfo.collider.name);
    }
}
