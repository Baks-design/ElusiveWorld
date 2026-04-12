using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraSwaying
    {
        readonly LookSettings settings;
        readonly Transform camTransform;
        float currentRawInput;
        float previousRawInput;
        float swayIntensity;
        bool changingDirection;

        public CameraSwaying(LookSettings settings, Transform camTransform)
        {
            this.settings = settings;
            this.camTransform = camTransform;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            currentRawInput = rawXInput;

            var isMoving = Mathf.Abs(currentRawInput) > 0.001f;
            if (isMoving)
            {
                var directionChanged =
                    Mathf.Sign(currentRawInput) != Mathf.Sign(previousRawInput) &&
                    Mathf.Abs(previousRawInput) > 0.001f;
                if (directionChanged) changingDirection = true;

                var multiplier = changingDirection
                    ? settings.changeDirectionMultiplier
                    : 1f;

                swayIntensity += inputVector.x * settings.swaySpeed * Time.deltaTime * multiplier;
            }
            else
            {
                if (Mathf.Abs(previousRawInput) < 0.001f) changingDirection = false;

                swayIntensity = swayIntensity.ExpDecay(0f, settings.returnSpeed, Time.deltaTime);
            }

            swayIntensity = Mathf.Clamp(swayIntensity, -1f, 1f);

            var curve = settings.swayCurve.Evaluate(Mathf.Abs(swayIntensity));
            var sway = curve * -settings.swayAmount * Mathf.Sign(swayIntensity);

            var euler = camTransform.localEulerAngles;
            camTransform.localRotation = Quaternion.Euler(euler.x, euler.y, sway);

            previousRawInput = currentRawInput;
        }
    }
}