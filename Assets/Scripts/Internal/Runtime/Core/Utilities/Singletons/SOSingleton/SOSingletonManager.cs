using System.Collections.Generic;
using UnityEngine;

namespace VHS
{
    public class SOSingletonManager : Singleton<SOSingletonManager>
    {
        [SerializeField] List<SOManager> managers;

        void OnEnable()
        {
            if (managers.Count == 0) return;
            foreach (SOManager so_manager in managers)
                if (so_manager != null)
                    so_manager.OnGameStart();
        }

        void OnDisable()
        {
            if (managers.Count == 0) return;
            foreach (SOManager so_manager in managers)
                if (so_manager != null)
                    so_manager.OnGameEnd();
        }
    }
}
