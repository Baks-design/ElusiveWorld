using System;
using Eflatun.SceneReference;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType SceneType;
    }
}