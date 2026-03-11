using System;
using System.Collections.Generic;
using System.Linq;
using ZLinq;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType sceneType)
            => Scenes.AsValueEnumerable().
                FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
    }
}