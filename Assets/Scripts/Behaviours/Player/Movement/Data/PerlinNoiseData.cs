using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement.Data
{
    public enum TransformTarget { Position, Rotation, Both }

    [CreateAssetMenu(fileName = "PerlinNoiseData", menuName = "Data/Behaviours/Movement/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget;
        public float amplitude;
        public float frequency;
    }
}