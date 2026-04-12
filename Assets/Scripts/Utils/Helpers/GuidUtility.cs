using System;
using System.Security.Cryptography;
using System.Text;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Helpers
{
    public static class GuidUtility
    {
        public static Guid CreateGuidFromString(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            using var md5 = MD5.Create();
            return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}