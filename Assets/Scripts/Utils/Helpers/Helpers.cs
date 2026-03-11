using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class Helpers
    {
        public static Guid CreateGuidFromString(string input) 
            => new(MD5.Create().ComputeHash(Encoding.Default.GetBytes(input)));

        public static void ChangeCursorState(CursorLockMode lockMode)
            => Cursor.lockState = lockMode;
    }
}