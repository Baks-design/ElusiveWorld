using System;
using UnityEngine;

namespace VHS
{
    [Serializable]
    public class CameraSwaying
    {
        [Header("Sway Settings")]
        [SerializeField] float swayAmount = 0f;
        [SerializeField] float swaySpeed = 0f;
        [SerializeField] float returnSpeed = 0f;
        [SerializeField] float changeDirectionMultiplier = 0f;
        [SerializeField] AnimationCurve swayCurve = new();
        Transform camTransform;
        float scrollSpeed;
        float xAmountThisFrame;
        float xAmountPreviousFrame;
        bool diffrentDirection;

        public void Init(Transform cam) => camTransform = cam;

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            var xAmount = inputVector.x;

            xAmountThisFrame = rawXInput;

            if (rawXInput != 0f) 
            {
                if (xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0) 
                    diffrentDirection = true;

                var speedMultiplier = diffrentDirection ? changeDirectionMultiplier : 1f;
                scrollSpeed += xAmount * swaySpeed * Time.deltaTime * speedMultiplier;
            }
            else 
            {
                if (xAmountThisFrame == xAmountPreviousFrame) 
                    diffrentDirection = false; 

                scrollSpeed = Mathf.Lerp(scrollSpeed, 0f, Time.deltaTime * returnSpeed);
            }

            scrollSpeed = Mathf.Clamp(scrollSpeed, -1f, 1f);

            float swayFinalAmount;
            if (scrollSpeed < 0f)
                swayFinalAmount = -swayCurve.Evaluate(scrollSpeed) * -swayAmount;
            else
                swayFinalAmount = swayCurve.Evaluate(scrollSpeed) * -swayAmount;

            Vector3 swayVector;
            swayVector.z = swayFinalAmount;

            camTransform.localEulerAngles = new Vector3(
                camTransform.localEulerAngles.x, camTransform.localEulerAngles.y, swayVector.z);

            xAmountPreviousFrame = xAmountThisFrame;
        }
    }
}
