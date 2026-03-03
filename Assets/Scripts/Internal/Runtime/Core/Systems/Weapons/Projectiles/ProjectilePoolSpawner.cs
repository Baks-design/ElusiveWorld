using System.Collections.Generic;
using Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles.Types;
using Assets.Scripts.Internal.Runtime.Core.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles
{
    public class ProjectilePoolSpawner : Singleton<ProjectilePoolSpawner>
    {
        [SerializeField] int defaultPoolCapacity = 10;
        [SerializeField] int defaultMaxPoolSize = 20;
        readonly Dictionary<GameObject, IObjectPool<Projectile>> pools = new();

        public Projectile SpawnProjectile(
            Projectile prefab, Vector3 position,
            Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            if (prefab == null) return null;
            var pool = GetOrCreatePool(prefab);
            var projectile = pool.Get();
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.transform.SetParent(parent);
            projectile.transform.localScale = scale;
            projectile.gameObject.SetActive(true);
            return projectile;
        }

        IObjectPool<Projectile> GetOrCreatePool(Projectile prefab)
        {
            if (!pools.ContainsKey(prefab.gameObject))
            {
                var pool = new ObjectPool<Projectile>(
                    () => CreateProjectile(prefab),
                    OnGetProjectile,
                    OnReleaseProjectile,
                    OnDestroyProjectile,
                    true,
                    defaultPoolCapacity,
                    defaultMaxPoolSize
                );
                pools[prefab.gameObject] = pool;
            }

            return pools[prefab.gameObject];
        }

        Projectile CreateProjectile(Projectile prefab)
        {
            Projectile projectile = Instantiate(prefab);
            projectile.gameObject.SetActive(false);
            projectile.SetPool(pools[prefab.gameObject]);
            DontDestroyOnLoad(projectile.gameObject);
            return projectile;
        }

        void OnGetProjectile(Projectile projectile) => projectile.gameObject.SetActive(true);

        void OnReleaseProjectile(Projectile projectile) => projectile.gameObject.SetActive(false);

        void OnDestroyProjectile(Projectile projectile) => Destroy(projectile.gameObject);

        public void PreWarmPool(Projectile prefab, int count)
        {
            var pool = GetOrCreatePool(prefab);

            var preWarmed = new List<Projectile>();
            for (var i = 0; i < count; i++)
                preWarmed.Add(pool.Get());

            foreach (var projectile in preWarmed)
                pool.Release(projectile);
        }

        public void ClearPools()
        {
            foreach (var pool in pools.Values)
                pool.Clear();

            pools.Clear();
        }
    }
}