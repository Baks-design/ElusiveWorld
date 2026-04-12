using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraZoom
    {
        private readonly MonoBehaviour mono;
        readonly LookSettings settings;
        readonly CinemachineCamera cam;
        IEnumerator changeFOVRoutine;
        IEnumerator changeRunFOVRoutine;
        bool running;
        bool zooming;
        float initFOV;

        public CameraZoom(MonoBehaviour mono, LookSettings settings, CinemachineCamera cam)
        {
            this.mono = mono;
            this.settings = settings;
            this.cam = cam;

            InitializeSettings();
        }

        void InitializeSettings() => initFOV = cam.Lens.FieldOfView;

        public void ChangeFOV()
        {
            if (running)
            {
                zooming = !zooming;
                return;
            }

            StopRoutine(changeRunFOVRoutine);
            StopRoutine(changeFOVRoutine);

            changeFOVRoutine = ChangeFOVRoutine();
            mono.StartCoroutine(changeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            var percent = 0f;
            var speed = 1f / settings.zoomTransitionDuration;
            var start = cam.Lens.FieldOfView;
            var target = zooming ? initFOV : settings.zoomFOV;

            zooming = !zooming;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var t = settings.zoomCurve.Evaluate(percent);
                cam.Lens.FieldOfView = start.Eerp(target, t);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning)
        {
            StopRoutine(changeFOVRoutine);
            StopRoutine(changeRunFOVRoutine);

            changeRunFOVRoutine = ChangeRunFOVRoutine(returning);
            mono.StartCoroutine(changeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var duration = returning ? settings.runReturnTransitionDuration : settings.runTransitionDuration;
            var speed = 1f / duration;
            var start = cam.Lens.FieldOfView;
            var target = returning ? initFOV : settings.runFOV;

            running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var t = settings.runCurve.Evaluate(percent);
                cam.Lens.FieldOfView = start.Eerp(target, t);
                yield return null;
            }
        }

        void StopRoutine(IEnumerator routine)
        {
            if (routine != null) mono.StopCoroutine(routine);
        }
    }
}