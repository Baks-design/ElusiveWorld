using System;
using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    [Serializable]
    public class RaycastConfig
    {
        [Header("Ground Check")]
        public LayerMask groundLayers = -1;
        public float groundCheckDistance = 0.1f;
        public float groundedThreshold = 0.15f;
        public Vector3 groundCheckOffset = Vector3.zero;

        [Header("Ceiling Check")]
        public float ceilingCheckDistance = 0.1f;
        public float ceilingRadius = 0.2f;

        [Header("Wall Check")]
        public float wallCheckDistance = 0.2f;
        public int wallCheckRays = 4;
        public float wallCheckAngle = 360f;

        [Header("Slope Check")]
        public float maxSlopeAngle = 45f;
        public float slopeCheckDistance = 0.5f;

        [Header("Step Check")]
        public float maxStepHeight = 0.3f;
        public float stepCheckDistance = 0.2f;

        [Header("Debug")]
        public bool drawGizmos = true;
        public Color groundGizmoColor = Color.green;
        public Color wallGizmoColor = Color.yellow;
        public Color slopeGizmoColor = Color.red;
    }
}