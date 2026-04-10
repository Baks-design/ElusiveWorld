using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraRotation
    {
        readonly LookSettings settings;
        readonly Transform yawTransform;
        readonly Transform pitchTransform;
        Quaternion targetYawRotation;
        Quaternion targetPitchRotation;
        Quaternion currentYawRotation;
        Quaternion currentPitchRotation;
        float desiredYaw;
        float desiredPitch;

        public CameraRotation(
            LookSettings settings,
            Transform yawTransform,
            Transform pitchTransform)
        {
            this.settings = settings;
            this.yawTransform = yawTransform;
            this.pitchTransform = pitchTransform;
        }

        public void Update(InputManager input, float dt)
        {
            CalculateRotation(input, dt);
            SmoothRotation(dt);
            ApplyRotation();
        }

        void CalculateRotation(InputManager input, float dt)
        {
            var look = input.LookAxis;

            desiredYaw += look.x * settings.sensitivity.x * dt;
            desiredPitch -= look.y * settings.sensitivity.y * dt;
            desiredPitch = Mathf.Clamp(desiredPitch, settings.lookAngleMinMax.x, settings.lookAngleMinMax.y);

            targetYawRotation = Quaternion.Euler(0f, desiredYaw, 0f);
            targetPitchRotation = Quaternion.Euler(desiredPitch, 0f, 0f);
        }

        void SmoothRotation(float dt)
        {
            currentYawRotation = currentYawRotation.ExpDecay(targetYawRotation, settings.smoothAmount.x, dt);
            currentPitchRotation = currentPitchRotation.ExpDecay(targetPitchRotation, settings.smoothAmount.y, dt);
        }

        void ApplyRotation()
        {
            yawTransform.rotation = currentYawRotation;
            pitchTransform.localRotation = currentPitchRotation;
        }
    }
}