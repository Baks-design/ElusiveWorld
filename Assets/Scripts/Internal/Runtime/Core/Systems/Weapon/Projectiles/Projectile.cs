using UnityEngine;

namespace VHS
{
    public abstract class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] protected ProjectileData projectileData;
        [SerializeField] protected LayerMask collisionLayers;
        protected float shotTimeStamp;
        protected Vector3 lastPosition;
        protected Vector3 currentDirection;
        protected Transform owner;

        public bool IsPoolable => !gameObject.activeSelf;
        public Transform Transform => transform;

        protected virtual void Awake() => CastData();
        protected abstract void CastData();

        protected virtual void OnEnable() => UpdateManager.OnUpdate += OnUpdate;
        protected virtual void OnDisable() => UpdateManager.OnUpdate -= OnUpdate;
        protected virtual void OnDestroy() => UpdateManager.OnUpdate -= OnUpdate;

        public virtual void OnUpdate(float deltaTime)
        {
            UpdatePosition(deltaTime);
            CheckForCollision();
            CheckForLifetime();
            UpdateLastPosition();
        }

        protected virtual void UpdatePosition(float _deltaTime) { }
        void UpdateLastPosition() => lastPosition = transform.position;

        public virtual void CheckForCollision()
        {
            if (Physics.Linecast(lastPosition, transform.position, out var hitInfo, collisionLayers))
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
