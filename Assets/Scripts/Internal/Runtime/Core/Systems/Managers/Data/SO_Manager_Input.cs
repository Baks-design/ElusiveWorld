using Assets.Scripts.Internal.Runtime.Core.Utilities.Singletons;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Managers.Data
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Data/Managers/InputManager")]
    public class SO_Manager_Input : SO_Singleton<SO_Manager_Input>
    {
        [field: SerializeField] public float DoubleTapThreshold { get; private set; } = 0.2f;
    }
}