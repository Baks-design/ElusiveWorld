using System;
using UnityEngine;

namespace VHS
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] WeaponData weaponData;
        [SerializeField] Projectile projectile;
        int currentAmmoCount;
        float timeSinceLastShot;
        bool duringReload;

        public WeaponData Data => weaponData;
        public Projectile Projectile => projectile;
        public WeaponProjectileAnchor ProjectileAnchor { get; private set; }
        public int CurrentAmmoCount
        {
            get => currentAmmoCount;
            set => currentAmmoCount = value;
        }
        public float TimeSinceLastShot
        {
            get => timeSinceLastShot;
            set => timeSinceLastShot = value;
        }
        public bool DuringReload
        {
            get => duringReload;
            set => duringReload = value;
        }

        public Action OnWeaponShootPressed = delegate { };
        public Action OnWeaponShootHeld = delegate { };
        public Action OnWeaponShootReleased = delegate { };
        public Action OnWeaponShootSucceed = delegate { };
        public Action OnWeaponReloadPressed = delegate { };
        public Action OnWeaponReloadCompleted = delegate { };

        public virtual void Awake()
        {
            InitData();
            GetComponents();
        }

        public virtual void InitData()
        {
            Data.Init();
            CurrentAmmoCount = Data.AmmoCount;
        }

        public virtual void GetComponents() => ProjectileAnchor = GetComponentInChildren<WeaponProjectileAnchor>();

        public virtual void OnShootButtonPressed() => OnWeaponShootPressed();
        public virtual void OnShootButtonHeld() => OnWeaponShootHeld();
        public virtual void OnShootButtonReleased() => OnWeaponShootReleased();
        public virtual void OnReloadButtonPressed() => OnWeaponReloadPressed();
    }

    public class WeaponComponent<T> : MonoBehaviour where T : WeaponComponent<T>
    {
        bool cached;
        Weapon weapon;

        public Weapon Weapon
        {
            get
            {
                if (!cached)
                {
                    cached = true;
                    weapon = GetComponentInParent<Weapon>();
                }
                return weapon;
            }
        }
    }
}
