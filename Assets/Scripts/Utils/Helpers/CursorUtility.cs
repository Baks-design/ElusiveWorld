using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class CursorUtility
    {
        public static void SetState(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = lockMode switch
            {
                CursorLockMode.Locked => false,
                _ => true
            };
        }
    }
}