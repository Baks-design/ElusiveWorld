using ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles.Base
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected ProjectileData projectileData;
        [SerializeField] protected LayerMask collisionLayers;
        protected float shotTimeStamp;
        protected Vector3 lastPosition;
        protected Vector3 currentDirection;
        protected Transform owner;
        protected RaycastHit hitInfo;
        protected IObjectPool<Projectile> pool;
        ProjectileDecalPoolSpawner decalPool;
        
        protected virtual void Awake() => CastData();
        protected abstract void CastData();

        void Start() => decalPool = IServiceLocator.Default.GetService<ProjectileDecalPoolSpawner>();

        void Update()
        {
            UpdatePosition(Time.deltaTime);
            CheckForCollision();
            CheckForLifetime();
            UpdateLastPosition();
        }

        protected virtual void UpdatePosition(float deltaTime) { }
        void UpdateLastPosition() => lastPosition = transform.position;

        public virtual void CheckForCollision()
        {
            if (Physics.Linecast(lastPosition, transform.position, out hitInfo, collisionLayers))
            {
                if (hitInfo.collider.transform.IsChildOf(transform) ||
                    hitInfo.collider.transform.IsChildOf(owner))
                    return;

                OnHit();
                decalPool.SpawnDecal(hitInfo);
                if (hitInfo.transform.TryGetComponent<IDamageReceiver>(out var damage))
                    damage.TakeDamage(projectileData.GeneralSettings.Damage);
            }
        }

        void CheckForLifetime()
        {
            if (Time.time > shotTimeStamp + projectileData.GeneralSettings.MaxLiveDuration)
                ReleaseToPool();
        }

        public virtual void OnFire(Transform owner) => Init(owner);

        void Init(Transform owner)
        {
            this.owner = owner;
            shotTimeStamp = Time.time;
            lastPosition = transform.position;
            currentDirection = transform.forward;
        }

        public virtual void OnHit() => ReleaseToPool();

        protected void ReleaseToPool() => pool?.Release(this);

        public void SetPool(IObjectPool<Projectile> pool) => this.pool = pool;
    }
}