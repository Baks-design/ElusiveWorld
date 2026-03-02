using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utilities
{
    public static class ExtensionMethods
    {
        public static void CallWithDelay(this MonoBehaviour mono, Action method, float delay) =>
            mono.StartCoroutine(CallWithDelayRoutine(method, delay));

        static IEnumerator CallWithDelayRoutine(Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method();
        }
    }
}
