using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraRotation
    {
        readonly LookSettings settings;
        readonly Transform yawTranform;
        readonly Transform pitchTranform;
        Quaternion targetYawRotation;
        Quaternion targetPitchRotation;
        Quaternion currentYawRotation;
        Quaternion currentPitchRotation;
        float desiredYaw;
        float desiredPitch;

        public CameraRotation(
            LookSettings settings,
            Transform yawTranform,
            Transform pitchTranform)
        {
            this.settings = settings;
            this.yawTranform = yawTranform;
            this.pitchTranform = pitchTranform;
        }

        public void Update(InputManager input)
        {
            CalculateRotation(input);
            SmoothRotation();
            ApplyRotation();
        }

        void CalculateRotation(InputManager input)
        {
            desiredYaw += input.LookAxis.x * settings.sensitivity.x * Time.deltaTime;
            desiredPitch -= input.LookAxis.y * settings.sensitivity.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, settings.lookAngleMinMax.x, settings.lookAngleMinMax.y);

            targetYawRotation = Quaternion.Euler(0f, desiredYaw, 0f);
            targetPitchRotation = Quaternion.Euler(desiredPitch, 0f, 0f);
        }

        void SmoothRotation()
        {
            currentYawRotation = currentYawRotation.ExpDecay(
                targetYawRotation, settings.smoothAmount.x, Time.deltaTime);
            currentPitchRotation = currentPitchRotation.ExpDecay(
                targetPitchRotation, settings.smoothAmount.y, Time.deltaTime);
        }

        void ApplyRotation()
        {
            yawTranform.rotation = currentYawRotation;
            pitchTranform.localRotation = currentPitchRotation;
        }
    }
}