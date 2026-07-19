using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public enum TransformTarget { Position, Rotation, Both }

    [CreateAssetMenu(fileName = "PerlinNoiseData", menuName = "Data/Behaviours/Characters/Look/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget = TransformTarget.Rotation;
        public float amplitude = 1f;
        public float frequency = 0.5f;
    }
}