using System.Collections;
using Assets.Scripts.Internal.Runtime.Core.Utils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons.Projectiles
{
    public class ProjectileDecalPoolSpawner : Singleton<ProjectileDecalPoolSpawner>
    {
        [SerializeField] DecalProjector decalProjector;
        [SerializeField] Material decalMaterial;
        [SerializeField] bool collisionCheck = false;
        [SerializeField] int initialCapacity = 10;
        [SerializeField] int maxCapacity = 20;
        [SerializeField] Vector3 decalSize = new(0.5f, 0.5f, 0.5f);
        [SerializeField] float fadeDuration = 5f;
        IObjectPool<DecalProjector> decalPool;

        void Start() => decalPool = new ObjectPool<DecalProjector>
        (
            () =>
            {
                decalProjector.material = decalMaterial;
                decalProjector.fadeFactor = 1f;
                decalProjector.fadeScale = 0.95f;
                decalProjector.startAngleFade = 0f;
                decalProjector.endAngleFade = 30f;
                return decalProjector;
            },
            decalProjector => decalProjector.gameObject.SetActive(true),
            decalProjector => decalProjector.gameObject.SetActive(false),
            decalProjector => Destroy(decalProjector.gameObject),
            collisionCheck,
            initialCapacity,
            maxCapacity
        );

        public void SpawnDecal(RaycastHit hit)
        {
            var projector = decalPool.Get();
            projector.transform.position = hit.point + hit.normal * 0.01f;

            var normalRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            projector.transform.rotation = normalRotation * randomRotation;

            projector.size = decalSize;

            StartCoroutine(FadeAndRelease(projector, fadeDuration));
        }

        IEnumerator FadeAndRelease(DecalProjector projector, float duration) //TODO: Change to Async
        {
            var time = 0f;
            var initialFade = projector.fadeFactor;

            while (time < duration)
            {
                if (projector == null) yield break;

                time += Time.deltaTime;
                var t = time / duration;
                projector.fadeFactor = Mathf.Lerp(initialFade, 0f, t);

                yield return null;
            }

            if (projector != null)
            {
                projector.fadeFactor = initialFade;
                decalPool.Release(projector);
            }
        }
    }
}