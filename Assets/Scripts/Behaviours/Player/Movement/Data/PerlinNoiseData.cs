using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement.Data
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