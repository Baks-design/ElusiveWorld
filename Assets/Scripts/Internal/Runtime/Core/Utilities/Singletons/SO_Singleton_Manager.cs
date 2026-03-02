using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utilities.Singletons
{
    public class SO_Singleton_Manager : Singleton<SO_Singleton_Manager>
    {
        [SerializeField] List<SO_Manager> so_managers = new();

        void OnEnable()
        {
            if (so_managers.Count == 0) return;

            foreach (var manager in so_managers)
                manager.OnGameStart();
        }

        void OnDisable()
        {
            if (so_managers.Count == 0) return;

            foreach (var smanager in so_managers)
                smanager.OnGameEnd();
        }
    }
}
