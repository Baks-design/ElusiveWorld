using UnityEngine;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [Serializable]
    public class LookSettings
    {
        [Header("Look Settings")]
        public Vector2 sensitivity;
        public Vector2 smoothAmount;
        public Vector2 lookAngleMinMax;

        [Header("Breathing Settings")]
        public PerlinNoiseData data;
        public bool x = true;
        public bool y = false;
        public bool z = false;

        [Header("Sway Settings")]
        public float swayAmount = 0f;
        public float swaySpeed = 0f;
        public float returnSpeed = 0f;
        public float changeDirectionMultiplier = 0f;
        public AnimationCurve swayCurve = new();

        [Header("Zoom Settings")]
        [Range(20f, 60f)] public float zoomFOV = 20f;
        public AnimationCurve zoomCurve = new();
        public float zoomTransitionDuration = 0f;
        [Space]
        [Range(60f, 100f)] public float runFOV = 60f;
        public AnimationCurve runCurve = new();
        public float runTransitionDuration = 0f;
        public float runReturnTransitionDuration = 0f;
    }
}