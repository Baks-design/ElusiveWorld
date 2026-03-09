using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Base;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Types
{
    public class EnemyHealthController : EntityHealthController
    {
        [SerializeField] float despawnTime = 2f;
        [SerializeField] float respawnDelay = 3f;
        CancellationToken DestroyToken => this.GetCancellationTokenOnDestroy();

        protected override void OnDamageTaken(float amount) => base.OnDamageTaken(amount);

        protected override void Die()
        {
            if (CanResurrect)
            {
                RespawnAsync(respawnDelay, DestroyToken).Forget();
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject, despawnTime);
                gameObject.SetActive(false);
            }
        }

        protected override void OnResurrected()
        {
            base.OnResurrected();
            gameObject.SetActive(true);
        }

        async UniTaskVoid RespawnAsync(float delay, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(delay), 
                    delayTiming: PlayerLoopTiming.FixedUpdate,
                    cancellationToken: token);

                if (!token.IsCancellationRequested && this != null && !gameObject.activeSelf)
                    Resurrect();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Respawn cancelled - object was destroyed");
            }
        }
    }
}