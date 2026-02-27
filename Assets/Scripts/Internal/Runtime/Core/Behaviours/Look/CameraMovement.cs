using System;
using Unity.Cinemachine;
using UnityEngine;

namespace VHS
{
    [Serializable]
    public class CameraMovement
    {
        [Header("Look Settings")]
        [SerializeField] Vector2 sensitivity;
        [SerializeField] Vector2 smoothAmount;
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 lookAngleMinMax;
        CameraInputData camInputData;
        Transform transform;
        Transform pitchTranform;
        float yaw;
        float pitch;
        float desiredYaw;
        float desiredPitch;

        public void Init(Transform transform, CameraInputData camInputData)
        {
            GetComponents(transform, camInputData);
            InitValues();
        }

        void GetComponents(Transform transform, CameraInputData camInputData)
        {
            this.transform = transform;
            this.camInputData = camInputData;
            pitchTranform = transform.GetChild(0).transform;
        }

        void InitValues()
        {
            yaw = transform.eulerAngles.y;
            desiredYaw = yaw;
        }

        public void Update()
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
        }

        void CalculateRotation()
        {
            desiredYaw += camInputData.InputVector.x * sensitivity.x * Time.deltaTime;
            desiredPitch -= camInputData.InputVector.y * sensitivity.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, lookAngleMinMax.x, lookAngleMinMax.y);
        }

        void SmoothRotation()
        {
            yaw = Mathf.Lerp(yaw, desiredYaw, smoothAmount.x * Time.deltaTime);
            pitch = Mathf.Lerp(pitch, desiredPitch, smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation()
        {
            transform.eulerAngles = new Vector3(0f, yaw, 0f);
            pitchTranform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }
}
