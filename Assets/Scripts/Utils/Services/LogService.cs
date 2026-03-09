using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Services
{
    public class LogService : IService
    {
        public void Log(string message, [CallerFilePath] string filePath = "")
            => LogInternal(UnityEngine.Debug.Log, filePath, message);

        public void LogWarning(string message, [CallerFilePath] string filePath = "")
            => LogInternal(UnityEngine.Debug.LogWarning, filePath, message);

        public void LogError(string message, [CallerFilePath] string filePath = "")
            => LogInternal(UnityEngine.Debug.LogError, filePath, message);

        void LogInternal(Action<string> logAction, string filePath, string message)
        {
            var invoker = Path.GetFileNameWithoutExtension(filePath);
            logAction.Invoke($"<b>[{invoker}]</b> {message}");
        }
    }
}
