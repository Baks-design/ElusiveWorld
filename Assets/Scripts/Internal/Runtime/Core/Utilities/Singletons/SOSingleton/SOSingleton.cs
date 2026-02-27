using UnityEngine;

namespace VHS
{
    public abstract class SOSingleton<T> : SOManager where T : SOManager
    {
        static T m_instance;

        public static T Instance
        {
            get
            {
                if (!m_instance)
                {
                    T[] m_results = Resources.FindObjectsOfTypeAll<T>();
                    if (m_results.Length > 0)
                        m_instance = m_results[0];
                }
                if (!m_instance)
                    m_instance = CreateInstance<T>();
                return m_instance;
            }
        }

        public override void OnGameStart()
        {
            if (m_instance == null)
                m_instance = this as T;
            else
                Destroy(this);
        }

        public override void OnGameEnd() { }
    }
}
