using UnityEngine;

namespace VHS
{
    public abstract class SOManager : ScriptableObject
    {
        public abstract void OnGameStart();
        public abstract void OnGameEnd();
    }
}
