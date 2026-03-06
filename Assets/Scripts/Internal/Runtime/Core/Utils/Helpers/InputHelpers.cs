using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Helpers
{
    public static class InputHelpers
    {
        public static void ChangeCursorState(CursorLockMode lockMode) => Cursor.lockState = lockMode;
    }
}
