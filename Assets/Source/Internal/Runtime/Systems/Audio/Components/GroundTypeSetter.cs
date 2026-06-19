using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components
{
    public enum GroundType
    {
        Concrete,
        Muddy
    }
    
    public class GroundTypeSetter : MonoBehaviour
    {
        [field: SerializeField] public GroundType GroundType { get; set; }
    }
}