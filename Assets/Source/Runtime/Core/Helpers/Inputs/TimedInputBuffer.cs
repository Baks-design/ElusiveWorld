using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    /* Uso:
        -> Jump Example(Bool Type)

        [SerializeField] TimedBoolInputBuffer jumpBuffer = new();

        void Update()
        {
            if (IsJumpPressedThisFrame()) jumpBuffer.Set();
            
            TryConsumeBufferedJump();
        }

        void IsJumpPressedThisFrame() => Keyboard.current[jumpKey].wasPressedThisFrame;

        void TryConsumeBufferedJump()
        {
            if (!isGrounded || !jumpBuffer.HasBuffer) return;

            var velocity = rb.linearVelocity;
            velocity.y = jumpVelocity;
            rb.linearVelocity = velocity;

            jumpBuffer.Consume();
        }
    */
    [Serializable]
    public class TimedInputBuffer<T>
    {
        [SerializeField, Min(0f)] float holdTime = 0.12f;
        [SerializeField] T bufferedValue;
        float expiryTime = float.NegativeInfinity;
        bool hasBufferedInput;

        public bool HasBuffer => hasBufferedInput && Time.unscaledTime <= expiryTime;
        public float HoldTime => holdTime;
        public T Value => bufferedValue;

        public void Set(T value)
        {
            bufferedValue = value;
            hasBufferedInput = true;
            expiryTime = Time.unscaledTime + holdTime;
        }

        public void Consume()
        {
            hasBufferedInput = false;
            expiryTime = float.NegativeInfinity;
            bufferedValue = default;
        }

        public bool TryConsume(out T value)
        {
            if (!HasBuffer)
            {
                value = default;
                return false;
            }

            value = bufferedValue;
            Consume();
            return true;
        }

        public void SetHoldTime(float seconds) => holdTime = Mathf.Max(0f, seconds);
    }
}