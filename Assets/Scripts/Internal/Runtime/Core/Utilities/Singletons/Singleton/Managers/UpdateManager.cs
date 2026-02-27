using System;
using UnityEngine;

namespace VHS
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public static Action<float> OnUpdate = delegate { };
        public static Action<float> OnFixedUpdate = delegate { };
        public static Action<float> OnLateUpdate = delegate { };

        void Update() => OnUpdate(Time.deltaTime);
        void FixedUpdate() => OnFixedUpdate(Time.fixedDeltaTime);
        void LateUpdate() => OnLateUpdate(Time.smoothDeltaTime);
    }
}
