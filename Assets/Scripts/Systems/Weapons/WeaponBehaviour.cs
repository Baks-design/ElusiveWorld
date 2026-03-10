using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons
{
    public class WeaponBehaviour : WeaponComponent<WeaponBehaviour>
    {
        ProjectilePoolSpawner projectilePool;

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

        void Start()
        {
            projectilePool = IServiceLocator.Default.GetService<ProjectilePoolSpawner>();

            Weapon.OnWeaponReloadStarted += OnWeaponReloadStarted;
            Weapon.OnWeaponReloadCompleted += OnWeaponReloadCompleted;
            Weapon.OnWeaponShootReleased += OnWeaponShootReleased;
            switch (Weapon.Data.TriggerType)
            {
                case WeaponTriggerType.PullRelease: Weapon.OnWeaponShootPressed += OnWeaponShootPressed; break;
                case WeaponTriggerType.Continous: Weapon.OnWeaponShootHeld += OnWeaponShootHeld; break;
            }
        }

        void OnDisable()
        {
            Weapon.OnWeaponReloadStarted -= OnWeaponReloadStarted;
            Weapon.OnWeaponReloadCompleted -= OnWeaponReloadCompleted;
            Weapon.OnWeaponShootReleased -= OnWeaponShootReleased;
            switch (Weapon.Data.TriggerType)
            {
                case WeaponTriggerType.PullRelease: Weapon.OnWeaponShootPressed -= OnWeaponShootPressed; break;
                case WeaponTriggerType.Continous: Weapon.OnWeaponShootHeld -= OnWeaponShootHeld; break;
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

            var projectileInstance = projectilePool.SpawnProjectile(
                Weapon.Projectile,
                Weapon.ProjectileAnchor.transform.position,
                Weapon.ProjectileAnchor.transform.rotation,
                Weapon.Projectile.transform.localScale
            );
            if (projectileInstance != null) projectileInstance.OnFire(transform.root);
        }

        public virtual void OnWeaponReloadStarted() => Weapon.DuringReload = true;

        public virtual void OnWeaponReloadCompleted()
        {
            Weapon.DuringReload = false;
            Weapon.CurrentAmmoCount = Weapon.Data.AmmoCount;
        }
    }
}