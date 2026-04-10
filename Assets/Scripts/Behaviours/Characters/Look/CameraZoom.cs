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

        public void ChangeFOV(MonoBehaviour mono, float dt)
        {
            if (running)
            {
                zooming = !zooming;
                return;
            }

            StopRoutine(mono, changeRunFOVRoutine);
            StopRoutine(mono, changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine(dt);
            mono.StartCoroutine(changeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine(float dt)
        {
            var percent = 0f;
            var speed = 1f / settings.zoomTransitionDuration;
            var start = cam.Lens.FieldOfView;
            var target = zooming ? initFOV : settings.zoomFOV;

            zooming = !zooming;

            while (percent < 1f)
            {
                percent += dt * speed;
                var t = settings.zoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = start.Eerp(target, t);
                yield return null;
            }
        }

        public void ChangeRunFOV(MonoBehaviour mono, bool returning, float dt)
        {
            StopRoutine(mono, changeFOVRoutine);
            StopRoutine(mono, changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning, dt);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning, float dt)
        {
            var percent = 0f;
            var duration = returning ? settings.runReturnTransitionDuration : settings.runTransitionDuration;
            var speed = 1f / duration;
            var start = cam.Lens.FieldOfView;
            var target = returning ? initFOV : settings.runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += dt * speed;
                var t = settings.runCurve.Evaluate(percent);
                cam.Lens.FieldOfView = start.Eerp(target, t);
                yield return null;
            }
        }

        void StopRoutine(MonoBehaviour mono, IEnumerator routine)
        {
            if (routine != null) mono.StopCoroutine(routine);
        }
    }
}