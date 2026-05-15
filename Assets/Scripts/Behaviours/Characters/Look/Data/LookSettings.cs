using UnityEngine;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [Serializable]
    public class LookSettings
    {
        [Header("Look Settings")]
        public Vector2 sensitivity = new(3f, 3f);
        public Vector2 smoothAmount = new(5f, 5f);
        public Vector2 lookAngleMinMax = new(-60f, 60f);

        [Header("Breathing Settings")]
        public PerlinNoiseData data;
        public bool x = true;
        public bool y = false;
        public bool z = false;

        [Header("Sway Settings")]
        public float swayAmount = 1.5f;
        public float swaySpeed = 3f;
        public float returnSpeed = 3f;
        public float changeDirectionMultiplier = 4f;
        public AnimationCurve swayCurve = new();

        [Header("Zoom Settings")]
        public float fovSmooth = 5f;
        [Range(20f, 60f)] public float zoomFOV = 40f;
        public AnimationCurve zoomCurve = new();
        public float zoomTransitionDuration = 0.25f;
        [Space]
        [Range(60f, 100f)] public float runFOV = 70f;
        public AnimationCurve runCurve = new();
        public float runTransitionDuration = 0.75f;
        public float runReturnTransitionDuration = 0.5f;
    }
}