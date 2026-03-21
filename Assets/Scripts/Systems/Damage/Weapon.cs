using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] float damage = 20f;
        [SerializeField] DamageType damageType = DamageType.Physical;
        [Header("Hit Detection")]
        [SerializeField] Collider hitCollider;
        [SerializeField] LayerMask targetLayers = -1;
        GameObject owner;
        bool isAttacking;

        public float GetDamage => damage;
        public DamageType GetDamageType => damageType;

        void Awake()
        {
            if (hitCollider != null) hitCollider.enabled = false;
        }

        public void SetOwner(GameObject owner) => this.owner = owner;

        public void BeginAttack()
        {
            isAttacking = true;
            if (hitCollider != null) hitCollider.enabled = true;
        }

        public void EndAttack()
        {
            isAttacking = false;
            if (hitCollider != null) hitCollider.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!isAttacking || other.gameObject == owner) return;

            if (((1 << other.gameObject.layer) & targetLayers) != 0)
            {
                if (other.TryGetComponent<IDamageable>(out var damageable))
                {
                    var damageInfo = new DamageInfo(damage, damageType, owner)
                    {
                        target = other.gameObject,
                        hitPoint = other.ClosestPoint(transform.position),
                        hitDirection = (other.transform.position - transform.position).normalized
                    };
                    damageable.TakeDamage(damageInfo);
                }
            }
        }
    }
}