using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraSwaying
    {
        readonly LookSettings settings;
        readonly Transform camTransform;
        Quaternion lastRotationOffset = Quaternion.identity;
        float currentInput;
        float previousInput;
        float swayIntensity;
        bool changingDirection;

        public CameraSwaying(LookSettings settings, Transform camTransform)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.camTransform = camTransform != null ? camTransform : throw new ArgumentNullException(nameof(camTransform));
        }

        public void Update(float inputX)
        {
            currentInput = inputX;

            var isMoving = Mathf.Abs(currentInput) > 0.001f;
            if (isMoving)
            {
                var directionChanged =
                    Mathf.Sign(currentInput) != Mathf.Sign(previousInput) &&
                    Mathf.Abs(previousInput) > 0.001f;
                changingDirection = directionChanged;

                var multiplier = changingDirection ? settings.changeDirectionMultiplier : 1f;
                swayIntensity += currentInput * settings.swaySpeed * Time.deltaTime * multiplier;
            }
            else
            {
                swayIntensity = swayIntensity.ExpDecay(0f, settings.returnSpeed, Time.deltaTime);
                changingDirection = false;
            }

            swayIntensity = Mathf.Clamp(swayIntensity, -1f, 1f);

            ApplySway();

            previousInput = currentInput;
        }

        void ApplySway()
        {
            camTransform.localRotation *= Quaternion.Inverse(lastRotationOffset);

            var curve = settings.swayCurve.Evaluate(Mathf.Abs(swayIntensity));
            var sway = curve * -settings.swayAmount * Mathf.Sign(swayIntensity);

            lastRotationOffset = Quaternion.Euler(0f, 0f, sway);
            camTransform.localRotation *= lastRotationOffset;
        }

        public void Reset()
        {
            camTransform.localRotation *= Quaternion.Inverse(lastRotationOffset);
            lastRotationOffset = Quaternion.identity;
            swayIntensity = 0f;
            changingDirection = false;
        }
    }
}