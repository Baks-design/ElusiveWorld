using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    [Serializable]
    public class CameraZoom
    {
        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)] float zoomFOV = 20f;
        [SerializeField] AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] float zoomTransitionDuration = 0.2f;
        [Header("Run Settings")]
        [SerializeField, Range(60f, 100f)] float runFOV = 70f;
        [SerializeField] AnimationCurve runCurve = AnimationCurve.EaseInOut(0f, 0f, 1, 1f);
        [SerializeField] float runTransitionDuration = 0.3f;
        [SerializeField] float runReturnTransitionDuration = 0.2f;
        [Header("Optional Settings")]
        [SerializeField] bool allowZoomWhileRunning = false;
        [SerializeField] bool smoothInterruption = true;
        CinemachineCamera cam;
        Coroutine activeFOVRoutine;
        float defaultFOV;
        bool isZoomed;
        bool isRunning;
        MonoBehaviour coroutineRunner;

        public bool IsZooming => isZoomed;
        public bool IsRunning => isRunning;
        public float CurrentFOV => cam != null ? cam.Lens.FieldOfView : defaultFOV;
        public float DefaultFOV => defaultFOV;

        public void Initialize(CinemachineCamera cam, MonoBehaviour runner)
        {
            this.cam = cam;
            
            coroutineRunner = runner;
            defaultFOV = cam.Lens.FieldOfView;
        }

        public void ToggleZoom()
        {
            if (isRunning && !allowZoomWhileRunning) return;

            StopAllTransitions();
            isZoomed = !isZoomed;

            var targetFOV = isZoomed ? zoomFOV : defaultFOV;
            var duration = isZoomed ? zoomTransitionDuration : zoomTransitionDuration;

            StartFOVTransition(targetFOV, duration, zoomCurve);
        }

        public void SetRunning(bool running)
        {
            if (isRunning == running) return;

            if (running && isZoomed && !allowZoomWhileRunning)
                isZoomed = false;

            StopAllTransitions();
            isRunning = running;

            var targetFOV = isRunning ? runFOV : (isZoomed ? zoomFOV : defaultFOV);
            var duration = isRunning ? runTransitionDuration : runReturnTransitionDuration;
            var curve = isRunning ? runCurve : zoomCurve;

            StartFOVTransition(targetFOV, duration, curve);
        }

        void StartFOVTransition(float targetFOV, float duration, AnimationCurve curve)
        {
            if (smoothInterruption && activeFOVRoutine != null) 
                coroutineRunner.StopCoroutine(activeFOVRoutine);

            activeFOVRoutine = coroutineRunner.StartCoroutine(SmoothFOVChange(targetFOV, duration, curve));
        }

        IEnumerator SmoothFOVChange(float targetFOV, float duration, AnimationCurve curve)
        {
            if (cam == null || duration <= 0f)
            {
                cam.Lens.FieldOfView = targetFOV;
                activeFOVRoutine = null;
                yield break;
            }

            var startFOV = cam.Lens.FieldOfView;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var curveValue = curve.Evaluate(t);

                cam.Lens.FieldOfView = Mathf.LerpUnclamped(startFOV, targetFOV, curveValue);

                yield return null;
            }

            cam.Lens.FieldOfView = targetFOV;
            activeFOVRoutine = null;
        }

        void StopAllTransitions()
        {
            if (activeFOVRoutine != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(activeFOVRoutine);
                activeFOVRoutine = null;
            }
        }

        public void ResetToDefault()
        {
            StopAllTransitions();
            isZoomed = false;
            isRunning = false;
            cam.Lens.FieldOfView = defaultFOV;
        }

        public void SetZoomFOV(float fov)
        {
            zoomFOV = Mathf.Clamp(fov, 20f, 60f);
            if (isZoomed && !isRunning)
            {
                StopAllTransitions();
                StartFOVTransition(zoomFOV, zoomTransitionDuration, zoomCurve);
            }
        }

        public void SetRunFOV(float fov)
        {
            runFOV = Mathf.Clamp(fov, 60f, 100f);
            if (isRunning)
            {
                StopAllTransitions();
                StartFOVTransition(runFOV, runTransitionDuration, runCurve);
            }
        }
    }
}