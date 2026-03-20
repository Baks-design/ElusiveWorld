using System;
using System.Threading;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Look
{
    [Serializable]
    public class CameraZoom
    {
        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)] float zoomFOV = 40f;
        [SerializeField] AnimationCurve zoomCurve = new();
        [SerializeField] float zoomTransitionDuration = 0.25f;
        [Header("Run Settings")]
        [SerializeField, Range(60f, 100f)] float runFOV = 60f;
        [SerializeField] AnimationCurve runCurve = new();
        [SerializeField] float runTransitionDuration = 0f;
        [SerializeField] float runReturnTransitionDuration = 0f;
        CancellationTokenSource fovCancellationSource;
        CancellationTokenSource runCancellationSource;
        CinemachineCamera cam;
        float initFOV;
        bool running;
        bool zooming;

        public void Init(CinemachineCamera cam)
        {
            this.cam = cam;
            initFOV = cam.Lens.FieldOfView;
        }

        public async Awaitable ChangeFOV()
        {
            if (running) return;

            if (runCancellationSource != null)
            {
                runCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }
            if (fovCancellationSource != null)
            {
                fovCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }

            fovCancellationSource = new CancellationTokenSource();
            await ChangeFOVRoutine(fovCancellationSource.Token);
        }

        async Awaitable ChangeFOVRoutine(CancellationToken cancellationToken = default)
        {
            var percent = 0f;
            var speed = 1f / zoomTransitionDuration;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = zooming ? initFOV : zoomFOV;

            zooming = !zooming;

            while (percent < 1f)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                percent += Time.deltaTime * speed;
                var smoothPercent = zoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);

                await Awaitable.NextFrameAsync();
            }
        }

        public async Awaitable ChangeRunFOV(bool returning)
        {
            if (fovCancellationSource != null)
            {
                fovCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }
            if (runCancellationSource != null)
            {
                runCancellationSource.Cancel();
                await Awaitable.NextFrameAsync();
            }

            runCancellationSource = new CancellationTokenSource();
            await ChangeRunFOVRoutine(returning, runCancellationSource.Token);
        }

        async Awaitable ChangeRunFOVRoutine(bool returning, CancellationToken cancellationToken = default)
        {
            var percent = 0f;
            var duration = returning ? runReturnTransitionDuration : runTransitionDuration;
            var speed = 1f / duration;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : runFOV;

            running = !returning;

            while (percent < 1f)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                percent += Time.deltaTime * speed;
                var smoothPercent = runCurve.Evaluate(percent);
                cam.Lens.FieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);

                await Awaitable.NextFrameAsync();
            }
        }
    }
}