using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] DamageType damageType = DamageType.Physical;
        [SerializeField] LayerMask targetLayers = -1;
        static readonly Collider[] colliderCache = new Collider[128];

        public void DealDamage(GameObject target)
        {
            if (target == null) return;

            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                var damageInfo = new DamageInfo(baseDamage, damageType, gameObject);
                damageable.TakeDamage(damageInfo);
            }
        }

        public void DealDamageWithInfo(DamageInfo damageInfo)
        {
            if (damageInfo.target == null) return;

            var damageable = damageInfo.target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damageInfo);
        }

        public void DealAreaDamage(Vector3 center, float radius, float damageMultiplier = 1f)
        {
            var hitCount = Physics.OverlapSphereNonAlloc(center, radius, colliderCache, targetLayers);
            if (hitCount == 0) return;

            var finalDamage = baseDamage * damageMultiplier;
            var damageInfo = new DamageInfo(finalDamage, damageType, gameObject);

            for (var i = 0; i < hitCount; i++)
            {
                var col = colliderCache[i];
                if (col == null) continue;
                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageInfo.target = col.gameObject;
                    damageable.TakeDamage(damageInfo);
                }
            }
            for (var i = 0; i < hitCount; i++)
                colliderCache[i] = null;
        }

        public void SetDamage(float damage) => baseDamage = Mathf.Max(0f, damage);

        public void SetDamageType(DamageType type) => damageType = type;
    }
}