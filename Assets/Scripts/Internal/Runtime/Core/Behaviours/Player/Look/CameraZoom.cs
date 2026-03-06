using System;
using LitMotion;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Look
{
    [Serializable]
    public class CameraZoom //TODO: Apply Extension
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
        CinemachineCamera cam;
        MotionHandle zoomHandle;
        MotionHandle runHandle;
        float initFOV;
        bool running;
        bool zooming;

        public void Init(CinemachineCamera cam)
        {
            this.cam = cam;
            initFOV = cam.Lens.FieldOfView;
        }

        public void ChangeFOV()
        {
            if (running)
            {
                zooming = !zooming;
                return;
            }

            if (zoomHandle.IsActive()) zoomHandle.Cancel();

            zooming = !zooming;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = zooming ? zoomFOV : initFOV;

            zoomHandle = LMotion.Create(currentFOV, targetFOV, zoomTransitionDuration)
                .WithEase(zoomCurve)
                .Bind(x => cam.Lens.FieldOfView = x);
        }

        public void ChangeRunFOV(bool returning)
        {
            if (zoomHandle.IsActive()) zoomHandle.Cancel();
            if (runHandle.IsActive()) runHandle.Cancel();

            var duration = returning ? runReturnTransitionDuration : runTransitionDuration;
            var currentFOV = cam.Lens.FieldOfView;
            var targetFOV = returning ? initFOV : runFOV;

            running = !returning;

            runHandle = LMotion.Create(currentFOV, targetFOV, duration)
                .WithEase(runCurve)
                .Bind(x => cam.Lens.FieldOfView = x);
        }
    }
}