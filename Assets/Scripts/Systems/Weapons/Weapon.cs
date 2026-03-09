using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class Weapon : MonoBehaviour
    {
        Vector3 currentAimPoint;
        float timeSinceLastShot;
        int currentAmmoCount;
        bool duringReload;

        [field: SerializeField] public WeaponData Data { get; private set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }
        public WeaponProjectileAnchor ProjectileAnchor { get; private set; }
        public int CurrentAmmoCount { get => currentAmmoCount; set => currentAmmoCount = value; }
        public float TimeSinceLastShot { get => timeSinceLastShot; set => timeSinceLastShot = value; }
        public bool DuringReload { get => duringReload; set => duringReload = value; }
        public Vector3 CurrentAimPoint { get => currentAimPoint; set => currentAimPoint = value; }

        public UnityAction OnWeaponShootPressed = delegate { };
        public UnityAction OnWeaponShootHeld = delegate { };
        public UnityAction OnWeaponShootReleased = delegate { };
        public UnityAction OnWeaponShootSucceed = delegate { };
        public UnityAction OnWeaponReloadPressed = delegate { };
        public UnityAction OnWeaponReloadStarted = delegate { };
        public UnityAction OnWeaponReloadCompleted = delegate { };

        public virtual void OnShootButtonPressed() => OnWeaponShootPressed();
        public virtual void OnShootButtonHeld() => OnWeaponShootHeld();
        public virtual void OnShootButtonReleased() => OnWeaponShootReleased();
        public virtual void OnReloadButtonPressed() => OnWeaponReloadPressed();

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
    }
}
