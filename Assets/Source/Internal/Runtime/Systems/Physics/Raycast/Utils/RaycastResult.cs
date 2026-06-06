using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public struct RaycastResult
    {
        public bool Hit;
        public Vector3 Point;
        public Vector3 Normal;
        public float Distance;
        public Collider Collider;
        public Transform HitTransform;

        public static RaycastResult Empty => new() { Hit = false };
    }
}