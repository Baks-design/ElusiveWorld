using UnityEngine;

namespace VHS
{
    public enum TransformTarget { Position, Rotation, Both }

    [CreateAssetMenu(menuName = "Data/PerlinNoiseData", order = 2)]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget;
        public float amplitude;
        public float frequency;
    }
}