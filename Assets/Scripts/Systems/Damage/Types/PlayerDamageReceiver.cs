using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Base;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Types
{
    public class PlayerHealthController : EntityHealthController
    {
        [SerializeField] float respawnDelay = 3f;
        CancellationToken DestroyToken => this.GetCancellationTokenOnDestroy();

        protected override void OnDamageTaken(float amount) => base.OnDamageTaken(amount);
        protected override void OnHealed(float amount) => base.OnHealed(amount);
        protected override void OnResurrected() => base.OnResurrected();

        protected override void Die()
        {
            Debug.Log("Player died");

            if (CanResurrect)
            {
                Debug.Log("Player can respawn with remaining lives");
                RespawnAsync(respawnDelay, DestroyToken).Forget();
            }
            else
            {
                Debug.Log("Game Over - No lives remaining");
                gameObject.SetActive(false);
            }
        }

        public override void Resurrect()
        {
            base.Resurrect();
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

                if (!token.IsCancellationRequested && this != null)
                    Resurrect();

            }
            catch (OperationCanceledException)
            {
                Debug.Log("Respawn cancelled - player object was destroyed");
            }
        }
    }
}