using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraRotation
    {
        readonly LookSettings settings;
        readonly Transform yawTransform;
        readonly Transform pitchTransform;
        readonly InputManager input;
        Quaternion targetYawRotation;
        Quaternion targetPitchRotation;
        Quaternion currentYawRotation;
        Quaternion currentPitchRotation;
        float desiredYaw;
        float desiredPitch;

        public CameraRotation(
            LookSettings settings,
            Transform yawTransform,
            Transform pitchTransform,
            InputManager input)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.yawTransform = yawTransform != null ? yawTransform : throw new ArgumentNullException(nameof(yawTransform));
            this.pitchTransform = pitchTransform != null ? pitchTransform : throw new ArgumentNullException(nameof(pitchTransform));
            this.input = input != null ? input : throw new ArgumentNullException(nameof(input));

            Initialize();
        }

        void Initialize()
        {
            currentYawRotation = yawTransform.rotation;
            currentPitchRotation = pitchTransform.localRotation;

            targetYawRotation = currentYawRotation;
            targetPitchRotation = currentPitchRotation;

            desiredYaw = yawTransform.eulerAngles.y;
            desiredPitch = NormalizeAngle(pitchTransform.localEulerAngles.x);
        }

        static float NormalizeAngle(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        public void Update()
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
        }

        void CalculateRotation()
        {
            var look = input.LookAxis;
            if (look.sqrMagnitude < 0.000001f) return;

            var delta = look;
            delta *= Time.deltaTime;

            desiredYaw += delta.x * settings.sensitivity.x;
            desiredYaw = Mathf.Repeat(desiredYaw, 360f);

            desiredPitch -= delta.y * settings.sensitivity.y;
            desiredPitch = Mathf.Clamp(desiredPitch, settings.lookAngleMinMax.x, settings.lookAngleMinMax.y);

            targetYawRotation = Quaternion.Euler(0f, desiredYaw, 0f);
            targetPitchRotation = Quaternion.Euler(desiredPitch, 0f, 0f);
        }

        void SmoothRotation()
        {
            currentYawRotation = currentYawRotation.ExpDecay(targetYawRotation, settings.smoothAmount.x, Time.deltaTime);
            currentPitchRotation = currentPitchRotation.ExpDecay(targetPitchRotation, settings.smoothAmount.y, Time.deltaTime);
        }

        void ApplyRotation()
        {
            yawTransform.rotation = currentYawRotation;
            pitchTransform.localRotation = currentPitchRotation;
        }
    }
}