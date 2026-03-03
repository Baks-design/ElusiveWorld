using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Types
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

        protected virtual void Awake() => CastData();
        protected abstract void CastData();

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

                ProjectileDecalPoolSpawner.Instance.SpawnDecal(hitInfo);
                OnHit();
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