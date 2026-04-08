using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraZoom //FIXME: TASKS
    {
        readonly LookSettings settings;
        readonly Camera cam;
        readonly float initFOV;
        CancellationTokenSource tokenChangeAimSource;
        CancellationTokenSource tokenChangeRunSource;
        bool running;

        public bool IsCanZooming { get; private set; } = true;
        public bool IsZooming { get; private set; }

        public CameraZoom(LookSettings settings, Camera cam)
        {
            this.settings = settings;
            this.cam = cam;

            initFOV = cam.fieldOfView;
        }

        public void Dispose()
        {
            tokenChangeAimSource?.Cancel();
            tokenChangeAimSource?.Dispose();
            tokenChangeRunSource?.Cancel();
            tokenChangeRunSource?.Dispose();
        }

        public void ChangeFOV()
        {
            if (!IsCanZooming) return;

            if (running)
            {
                IsZooming = !IsZooming;
                return;
            }

            tokenChangeRunSource?.Cancel();
            tokenChangeRunSource?.Dispose();
            tokenChangeAimSource?.Cancel();
            tokenChangeAimSource?.Dispose();

            tokenChangeAimSource = new();

            ChangeFOVRoutine(tokenChangeAimSource.Token).Forget();
        }

        async UniTask ChangeFOVRoutine(CancellationToken source)
        {
            var percent = 0f;
            var speed = 1f / settings.zoomTransitionDuration;
            var currentFOV = cam.fieldOfView;
            var targetFOV = IsZooming ? initFOV : settings.zoomFOV;

            IsZooming = !IsZooming;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = settings.zoomCurve.Evaluate(percent);
                cam.fieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);
                await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, source);
            }
        }

        public void ChangeRunFOV(bool returning)
        {
            tokenChangeAimSource?.Cancel();
            tokenChangeAimSource?.Dispose();
            tokenChangeRunSource?.Cancel();
            tokenChangeRunSource?.Dispose();

            tokenChangeRunSource = new();

            ChangeRunFOVTask(tokenChangeRunSource.Token, returning).Forget();
        }

        async UniTask ChangeRunFOVTask(CancellationToken source, bool returning)
        {
            var percent = 0f;
            var duration = returning ? settings.runReturnTransitionDuration : settings.runTransitionDuration;
            var speed = 1f / duration;
            var currentFOV = cam.fieldOfView;
            var targetFOV = returning ? initFOV : settings.runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = settings.runCurve.Evaluate(percent);
                cam.fieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);
                await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, source);
            }
        }
    }
}