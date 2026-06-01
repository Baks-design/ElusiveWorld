using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [Serializable]
    public class MovementSettings
    {
        [Header("Locomotion Settings")]
        public float crouchSpeed = 1f;
        public float walkSpeed = 3f;
        public float runSpeed = 6f;
        public float jumpSpeed = 6f;
        public float jumpHeight = 2f;
        public float slideSpeed = 10f;
        public float airControl = 10f;
        [Range(0f, 1f)] public float moveBackwardsSpeedPercent = 0.8f;
        [Range(0f, 1f)] public float moveSideSpeedPercent = 0.9f;

        [Header("Run Settings")]
        [Range(-1f, 1f)] public float canRunThreshold = -0.1f;
        public AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Crouch Settings")]
        public float crouchSmooth = 10f;
        [Range(0.2f, 0.9f)] public float crouchPercent = 0.6f;
        public float crouchTransitionDuration = 0.5f;
        public AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Slide Settings")]
        [Range(0.2f, 0.9f)] public float slidePercent = 0.3f;
        public float slideTransitionDuration = 0.3f;
        public float maxSlideDuration = 1.25f;
        public AnimationCurve slideTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Landing Settings")]
        [Range(0.05f, 0.5f)] public float lowLandAmount = 0.1f;
        [Range(0.2f, 0.9f)] public float highLandAmount = 0.4f;
        public float landTimer = 0.5f;
        public float landDuration = 0.5f;
        public AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Gravity Settings")]
        public float gravityMultiplier = 2.5f;
        public float stickToGroundForce = 1f;
        public LayerMask groundLayer = ~0;
        [Range(0f, 1f)] public float rayLength = 0.1f;
        [Range(0.01f, 1f)] public float raySphereRadius = 0.2f;

        [Header("Slope Settings")]
        public float slopeGravity = 20f;
        public float slopeAcceleration = 8f;
        public float maxSlopeSpeed = 10f;

        [Header("Check Wall Settings")]
        public LayerMask obstacleLayers = ~0;
        [Range(0f, 1f)] public float rayObstacleLength = 0.4f;
        [Range(0.01f, 1f)] public float rayObstacleSphereRadius = 0.2f;

        [Header("Smooth Settings")]
        [Range(1f, 100f)] public float smoothRotateSpeed = 10f;
        [Range(1f, 100f)] public float smoothInputSpeed = 10f;
        [Range(1f, 100f)] public float smoothVelocitySpeed = 3f;
        [Range(1f, 100f)] public float smoothFinalDirectionSpeed = 10f;
        [Range(1f, 100f)] public float smoothHeadBobSpeed = 5f;
    }
}