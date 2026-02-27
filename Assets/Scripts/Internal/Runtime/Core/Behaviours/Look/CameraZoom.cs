using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace VHS
{
    [Serializable]
    public class CameraZoom
    {
        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)] float zoomFOV = 20f;
        [SerializeField] AnimationCurve zoomCurve = new();
        [SerializeField] float zoomTransitionDuration = 0f;
        [Header("Run Settings")]
        [SerializeField, Range(60f, 100f)] float runFOV = 60f;
        [SerializeField] AnimationCurve runCurve = new();
        [SerializeField] float runTransitionDuration = 0f;
        [SerializeField] float runReturnTransitionDuration = 0f;
        CameraInputData inputData;
        IEnumerator changeFOVRoutine;
        IEnumerator changeRunFOVRoutine;
        CinemachineCamera cam;
        float initFOV;
        bool running;
        bool zooming;

        public void Init(CinemachineCamera cam, CameraInputData inputData)
        {
            this.cam = cam;
            this.inputData = inputData;
            initFOV = cam.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (running)
            {
                inputData.IsZooming = !inputData.IsZooming;
                zooming = inputData.IsZooming;
                return;
            }

            if (changeRunFOVRoutine != null) mono.StopCoroutine(changeRunFOVRoutine);
            if (changeFOVRoutine != null) mono.StopCoroutine(changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine();
            mono.StartCoroutine(changeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            var percent = 0f;
            var speed = 1f / zoomTransitionDuration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = inputData.IsZooming ? initFOV : zoomFOV;

            inputData.IsZooming = !inputData.IsZooming;
            zooming = inputData.IsZooming;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = zoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning, MonoBehaviour mono)
        {
            if (changeFOVRoutine != null) mono.StopCoroutine(changeFOVRoutine);
            if (changeRunFOVRoutine != null) mono.StopCoroutine(changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var duration = returning ? runReturnTransitionDuration : runTransitionDuration;
            var speed = 1f / duration;

            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = runCurve.Evaluate(percent);
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}