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

            var isMoving = Mathf.Abs(rawXInput) > 0.001f;

            if (isMoving)
            {
                var hasDirectionChanged =
                    Mathf.Sign(currentRawInput) !=
                    Mathf.Sign(previousRawInput) &&
                    Mathf.Abs(previousRawInput) > 0.001f;

                if (hasDirectionChanged) changingDirection = true;

                var speedMultiplier = changingDirection ? settings.changeDirectionMultiplier : 1f;
                swayIntensity += inputVector.x * settings.swaySpeed * Time.deltaTime * speedMultiplier;
            }
            else
            {
                if (!isMoving && Mathf.Abs(previousRawInput) < 0.001f) changingDirection = false;

                swayIntensity = swayIntensity.ExpDecay(0f, settings.returnSpeed, Time.deltaTime);
            }

            swayIntensity = Mathf.Clamp(swayIntensity, -1f, 1f);

            var curveValue = settings.swayCurve.Evaluate(Mathf.Abs(swayIntensity));
            var swayAmount = curveValue * -settings.swayAmount * Mathf.Sign(swayIntensity);

            var currentRotation = camTransform.localEulerAngles;
            camTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, swayAmount);

            previousRawInput = currentRawInput;
        }
    }
}