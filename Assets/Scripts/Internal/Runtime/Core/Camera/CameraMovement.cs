using System;
using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    [Serializable]
    public class CameraMovement
    {
        [SerializeField] float horizontalSensitivity = 2f;
        [SerializeField] float verticalSensitivity = 2f;
        [SerializeField] float pitchMin = -80f;
        [SerializeField] float pitchMax = 80f;
        [SerializeField] float smoothTime = 0.1f;
        Transform targetTransform;
        float currentYaw;
        float currentPitch;
        float targetYaw;
        float targetPitch;
        float currentYawVelocity;
        float currentPitchVelocity;

        public void Initialize(Transform targetTransform, Transform target)
        {
            this.targetTransform = targetTransform;

            targetTransform.position = target.position;

            var currentRotation = targetTransform.eulerAngles;
            currentYaw = currentRotation.y;
            currentPitch = currentRotation.x;
            targetYaw = currentYaw;
            targetPitch = currentPitch;
        }

        public void UpdateRotation(CameraInput input)
        {
            var inputYaw = input.Look.x * horizontalSensitivity;
            var inputPitch = input.Look.y * verticalSensitivity;

            targetYaw += inputYaw * Time.deltaTime;
            targetPitch -= inputPitch * Time.deltaTime;

            targetPitch = Mathf.Clamp(targetPitch, pitchMin, pitchMax);

            currentYaw = Mathf.SmoothDamp(currentYaw, targetYaw, ref currentYawVelocity, smoothTime);
            currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref currentPitchVelocity, smoothTime);

            targetTransform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }

        public void UpdatePosition(Transform target) => targetTransform.position = target.position;
    }
}