using System;
using Eflatun.SceneReference;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
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
        public SceneReference Reference;
        public SceneType SceneType;

        public string Name => Reference.Name;
    }
}