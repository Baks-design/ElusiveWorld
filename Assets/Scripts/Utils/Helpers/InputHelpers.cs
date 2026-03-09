using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class InputHelpers
    {
        public static void ChangeCursorState(CursorLockMode lockMode) => Cursor.lockState = lockMode;
    }
}
