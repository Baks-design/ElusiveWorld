using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utilities.Singletons
{
    public abstract class SO_Singleton<T> : SO_Manager where T : SO_Manager
    {
        static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    var results = Resources.FindObjectsOfTypeAll<T>();
                    if (results.Length > 0)
                        instance = results[0];
                }
                if (!instance)
                    instance = CreateInstance<T>();
                return instance;
            }
        }

        public override void OnGameStart()
        {
            if (instance == null)
                instance = this as T;
            else
                Destroy(this);
        }

        public override void OnGameEnd() { }
    }

    public abstract class SO_Manager : ScriptableObject
    {
        public abstract void OnGameStart();
        public abstract void OnGameEnd();
    }
}
