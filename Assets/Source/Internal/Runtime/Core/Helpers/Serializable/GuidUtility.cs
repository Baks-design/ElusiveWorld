using System;
using System.Security.Cryptography;
using System.Text;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class GuidUtility
    {
        public static Guid CreateGuidFromString(string input) =>
            new(MD5.Create().ComputeHash(Encoding.Default.GetBytes(input)));
    }
}