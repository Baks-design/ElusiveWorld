using UnityEngine;
using System;
using Unity.Cinemachine;
using System.Collections;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraZoom
    {
        readonly LookSettings settings;
        readonly CinemachineCamera cam;
        readonly float initFOV;
        IEnumerator changeFOVRoutine;
        IEnumerator changeRunFOVRoutine;
        bool running;
        bool zooming;

        public CameraZoom(LookSettings settings, CinemachineCamera cam)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.cam = cam != null ? cam : throw new ArgumentNullException(nameof(cam));

            initFOV = cam.Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (running)
            {
                zooming = !zooming;
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
            var speed = 1f / settings.zoomTransitionDuration;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = zooming ? initFOV : settings.zoomFOV;

            zooming = !zooming;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = settings.zoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(MonoBehaviour mono, bool returning)
        {
            if (changeFOVRoutine != null) mono.StopCoroutine(changeFOVRoutine);
            if (changeRunFOVRoutine != null) mono.StopCoroutine(changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var duration = returning ? settings.runReturnTransitionDuration : settings.runTransitionDuration;
            var speed = 1f / duration;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : settings.runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var smoothPercent = settings.runCurve.Evaluate(percent);
                cam.Lens.FieldOfView = currentFOV.Eerp(targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}