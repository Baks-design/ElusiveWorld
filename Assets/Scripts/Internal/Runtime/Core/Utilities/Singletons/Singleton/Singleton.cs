
using UnityEngine;

namespace VHS
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    var inst = new GameObject(typeof(T).Name);
                    instance = inst.AddComponent<T>();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
                instance = this as T;
            else
                Destroy(gameObject);
        }
    }
}
