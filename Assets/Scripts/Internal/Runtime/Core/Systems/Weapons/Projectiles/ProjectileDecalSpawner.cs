using Assets.Scripts.Internal.Runtime.Core.Utils;
using LitMotion;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles
{
    public class ProjectileDecalPoolSpawner : Singleton<ProjectileDecalPoolSpawner>
    {
        [SerializeField] Material decalMaterial;
        [SerializeField] Vector3 decalSize = new(0.5f, 0.5f, 0.5f);
        [SerializeField] float fadeDuration = 5f;
        IObjectPool<DecalProjector> decalPool;

        void Start() => decalPool = new ObjectPool<DecalProjector>
        (
            createFunc: () =>
            {
                var go = new GameObject("DecalProjector");
                var dp = go.AddComponent<DecalProjector>();
                go.transform.parent = transform;
                dp.material = decalMaterial;
                dp.fadeFactor = 1f;
                dp.fadeScale = 0.95f;
                dp.startAngleFade = 0f;
                dp.endAngleFade = 30f;
                return dp;
            },
            actionOnGet: dp => dp.gameObject.SetActive(true),
            actionOnRelease: dp => dp.gameObject.SetActive(false),
            actionOnDestroy: dp => Destroy(dp.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );

        public void SpawnDecal(RaycastHit hit)
        {
            var projector = decalPool.Get();
            projector.transform.position = hit.point + hit.normal * 0.01f;

            var normalRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            projector.transform.rotation = normalRotation * randomRotation;

            projector.size = decalSize;

            FadeAndRelease(projector, fadeDuration);
        }

        void FadeAndRelease(DecalProjector projector, float duration)
        {
            if (projector == null) return;

            var initialFade = projector.fadeFactor;

            LMotion.Create(initialFade, 0f, duration)
                .WithOnComplete(() =>
                {
                    if (projector != null)
                    {
                        projector.fadeFactor = initialFade;
                        decalPool.Release(projector);
                    }
                })
                .Bind(x => projector.fadeFactor = x);
        }
    }
}