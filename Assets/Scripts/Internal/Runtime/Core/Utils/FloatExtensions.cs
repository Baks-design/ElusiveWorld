using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils
{
    public static class FloatExtensions
    {
        public static float SmoothFactor(float value) => 1f - Mathf.Exp(-value * Time.deltaTime);
    }
}