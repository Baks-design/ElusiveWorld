using Assets.Scripts.Internal.Runtime.Core.Utils.Helpers;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] InputReader input;

        void Awake()
        {
            input.Init();
            input.EnableGameplay();
            InputHelpers.ChangeCursorState(CursorLockMode.Locked);
        }

        void OnDestroy()
        {
            input.DisableAllMaps();
            input.Dispose();
        }
    }
}
