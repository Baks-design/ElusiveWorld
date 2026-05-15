using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class CursorUtility
    {
        public static void SetState(CursorLockMode lockMode, bool isVisible)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = isVisible;
        }
    }
}