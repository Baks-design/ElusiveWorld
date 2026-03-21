using System;
using System.Collections.Generic;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            for (var i = 0; i < Scenes.Count; i++)
            {
                var scene = Scenes[i];
                if (scene.SceneType == sceneType) return scene.ScenePath;
            }
            return null;
        }
    }
}
