using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/MovementInputData", order = 1)]
    public class MovementInputData : ScriptableObject
    {
        Vector2 inputVector;
        bool isCrouching;
        bool isJumping;
        bool isRunning;

        public bool HasInput => inputVector != Vector2.zero;
        public Vector2 InputVector
        {
            get => inputVector;
            set => inputVector = value;
        }
        public bool IsCrouching
        {
            get => isCrouching;
            set => isCrouching = value;
        }
        public bool IsJumping
        {
            get => isJumping;
            set => isJumping = value;
        }
        public bool IsRunning
        {
            get => isRunning;
            set => isRunning = value;
        }

        public void ResetInput()
        {
            inputVector = Vector2.zero;
            isRunning = false;
            isCrouching = false;
            isJumping = false;
        }
    }
}