using UnityEditor;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement.Editor
{
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sceneLoader = (SceneLoader)target;
            if (EditorApplication.isPlaying && GUILayout.Button("Load First Scene Group"))
                LoadSceneGroup(sceneLoader, 0);
            if (EditorApplication.isPlaying && GUILayout.Button("Load Second Scene Group"))
                LoadSceneGroup(sceneLoader, 1);
        }

        static async void LoadSceneGroup(SceneLoader sceneLoader, int index) 
            => await sceneLoader.LoadSceneGroup(index);
    }
}