using Assets.Scripts.Internal.Runtime.Core.Systems.Managers;
using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Data;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles
{
    public abstract class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] protected ProjectileData projectileData;
        [SerializeField] protected LayerMask collisionLayers;
        protected float shotTimeStamp;
        protected Vector3 lastPosition;
        protected Vector3 currentDirection;
        protected Transform owner;
        protected RaycastHit hitInfo;

        public virtual bool IsPoolable => !gameObject.activeSelf;
        public Transform Transform => transform;

        protected virtual void Awake() => CastData();
        protected abstract void CastData();

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        void Update() => OnUpdate();
        public virtual void OnUpdate()
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
            }
        }

        void CheckForLifetime()
        {
            if (Time.time > shotTimeStamp + projectileData.GeneralSettings.MaxLiveDuration)
                gameObject.SetActive(false);
        }

        public virtual void OnFire(Transform owner) => Init(owner);

        void Init(Transform owner)
        {
            this.owner = owner;
            shotTimeStamp = Time.time;
            lastPosition = transform.position;
            currentDirection = transform.forward;
        }

        public virtual void OnHit() => gameObject.SetActive(false);

        public IPoolable OnReuse() => this;
    }
}
