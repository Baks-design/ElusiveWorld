using UnityEngine;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraZoom
    {
        readonly LookSettings settings;
        readonly CinemachineCamera cam;
        readonly float velocity;
        float baseFOV;
        float currentFOV;
        float targetFOV;
        bool isZooming;
        bool isRunning;

        public CameraZoom(LookSettings settings, CinemachineCamera cam)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.cam = cam != null ? cam : throw new ArgumentNullException(nameof(cam));

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            baseFOV = cam.Lens.FieldOfView;
            currentFOV = baseFOV;
            targetFOV = baseFOV;
        }

        public void Update()
        {
            currentFOV = currentFOV.Eerp(targetFOV, settings.fovSmooth * Time.deltaTime);
            cam.Lens.FieldOfView = currentFOV;
        }

        public void ToggleZoom()
        {
            if (isRunning) return;

            isZooming = !isZooming;
            UpdateTargetFOV();
        }

        public void SetRunning(bool running)
        {
            isRunning = running;
            UpdateTargetFOV();
        }

        void UpdateTargetFOV()
        {
            if (isRunning)
                targetFOV = settings.runFOV;
            else if (isZooming)
                targetFOV = settings.zoomFOV;
            else
                targetFOV = baseFOV;
        }

        public void SetBaseFOV(float value)
        {
            baseFOV = value;
            UpdateTargetFOV();
        }
    }
}