using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public enum SceneLoadType
    {
        Default,
        Addressable
    }

    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        UserInterface,
        HUD,
        Cinematic,
        Environment,
        Tooling
    }

    [Serializable]
    public class SceneData
    {
#if UNITY_EDITOR
        public SceneAsset Reference;
#endif
        public SceneLoadType SceneLoadType;
        public SceneType SceneType;

        public string ScenePath
        {
            get
            {
#if UNITY_EDITOR
                return Reference != null ? Reference.GetEntityId().ToString() : string.Empty;
#else
                return string.Empty;
#endif
            }
        }
        public string Name
        {
            get
            {
#if UNITY_EDITOR
                return Reference != null ? Reference.name : string.Empty;
#else
                return string.Empty;
#endif
            }
        }
    }
}