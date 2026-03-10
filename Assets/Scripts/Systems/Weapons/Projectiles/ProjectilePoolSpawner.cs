using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles.Base;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles
{
    public class ProjectilePoolSpawner : MonoBehaviour, IService
    {
        readonly Dictionary<GameObject, IObjectPool<Projectile>> pools = new();

        public void Initialize() => IServiceLocator.Default.TryRegisterService(this);

        public void Dispose() => IServiceLocator.Default.TryUnregisterService(this);

        public Projectile SpawnProjectile(
            Projectile prefab, Vector3 position,
            Quaternion rotation, Vector3 scale)
        {
            var pool = GetOrCreatePool(prefab);
            var projectile = pool.Get();
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.transform.SetParent(transform);
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
                    false,
                    10,
                    20
                );
                pools[prefab.gameObject] = pool;
            }

            return pools[prefab.gameObject];
        }

        Projectile CreateProjectile(Projectile prefab)
        {
            var projectile = Instantiate(prefab);
            projectile.gameObject.SetActive(false);
            projectile.SetPool(pools[prefab.gameObject]);
            DontDestroyOnLoad(projectile.gameObject);
            return projectile;
        }

        void OnGetProjectile(Projectile projectile) => projectile.gameObject.SetActive(true);

        void OnReleaseProjectile(Projectile projectile) => projectile.gameObject.SetActive(false);

        void OnDestroyProjectile(Projectile projectile) => Destroy(projectile.gameObject);
    }
}