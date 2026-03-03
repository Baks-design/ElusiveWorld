using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] InputReader input;

        void Awake() => input.Init();

        void OnDestroy() => input.Dispose();
    }
}
