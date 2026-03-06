using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public bool AutoUnparentOnAwake = true;
#pragma warning disable UDR0001 
        protected static T instance;
#pragma warning restore UDR0001 

        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (AutoUnparentOnAwake)
                transform.SetParent(null);

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                    Destroy(gameObject);
            }
        }
    }
}