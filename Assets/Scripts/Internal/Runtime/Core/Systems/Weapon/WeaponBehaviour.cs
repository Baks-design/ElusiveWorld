using UnityEngine;

namespace VHS
{
    public class WeaponBehaviour : WeaponComponent<WeaponBehaviour>
    {
        protected bool CanShootWeapon
        {
            get
            {
                if (Weapon.DuringReload)
                    return false;
                if (Weapon.TimeSinceLastShot + Weapon.Data.TimeBetweenRounds <
                        Time.time && Weapon.CurrentAmmoCount > 0)
                    return true;
                return false;
            }
        }

        void OnEnable()
        {
            Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            Weapon.OnWeaponShootReleased += OnWeaponShootReleased;
            Weapon.OnWeaponReloadCompleted += OnWeaponReloadCompleted;
            switch (Weapon.Data.TriggerType)
            {
                case WeaponTriggerType.PullRelease:
                    Weapon.OnWeaponShootPressed += OnWeaponShootPressed;
                    break;
                case WeaponTriggerType.Continous:
                    Weapon.OnWeaponShootHeld += OnWeaponShootHeld;
                    break;
            }
        }

        void OnDisable()
        {
            Weapon.OnWeaponReloadPressed -= OnWeaponReloadPressed;
            Weapon.OnWeaponShootReleased -= OnWeaponShootReleased;
            Weapon.OnWeaponReloadCompleted -= OnWeaponReloadCompleted;
            switch (Weapon.Data.TriggerType)
            {
                case WeaponTriggerType.PullRelease:
                    Weapon.OnWeaponShootPressed -= OnWeaponShootPressed;
                    break;
                case WeaponTriggerType.Continous:
                    Weapon.OnWeaponShootHeld -= OnWeaponShootHeld;
                    break;
            }
        }

        void OnWeaponShootHeld()
        {
            if (!CanShootWeapon) return;
            OnWeaponShot();
        }

        void OnWeaponShootPressed()
        {
            if (!CanShootWeapon) return;
            OnWeaponShot();
        }

        void OnWeaponShootReleased() { }

        void OnWeaponShot()
        {
            Weapon.OnWeaponShootSucceed();

            Weapon.CurrentAmmoCount--;
            Weapon.TimeSinceLastShot = Time.time;

            var poolableProjectile = PoolManager.SpawnPoolable(
                Weapon.Projectile,
                Weapon.ProjectileAnchor.transform.position,
                Weapon.ProjectileAnchor.transform.rotation,
                Weapon.Projectile.transform.localScale
            );
            var projectileInstance = poolableProjectile.Transform.gameObject.GetComponent<Projectile>();
            projectileInstance.OnFire(transform.root);
        }

        public virtual void OnWeaponReloadPressed()
        {
            Weapon.DuringReload = true;
            Weapon.CurrentAmmoCount = Weapon.Data.AmmoCount;
        }

        public virtual void OnWeaponReloadCompleted() => Weapon.DuringReload = false;
    }
}
