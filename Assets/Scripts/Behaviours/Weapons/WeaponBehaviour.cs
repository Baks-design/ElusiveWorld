using ElusiveWorld.Core.Assets.Scripts.Behaviours.Weapons.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Weapons
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

        void Start()
        {
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
        }

        public virtual void OnWeaponReloadStarted() => Weapon.DuringReload = true;

        public virtual void OnWeaponReloadCompleted()
        {
            Weapon.DuringReload = false;
            Weapon.CurrentAmmoCount = Weapon.Data.AmmoCount;
        }
    }
}