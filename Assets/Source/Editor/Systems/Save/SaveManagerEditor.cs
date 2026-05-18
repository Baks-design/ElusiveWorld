using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using UnityEditor;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Save
{
    [CustomEditor(typeof(PersistenceManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var saveLoadSystem = (PersistenceManager)target;
            var gameName = saveLoadSystem.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game")) saveLoadSystem.NewGame();
            if (GUILayout.Button("Save Game")) saveLoadSystem.SaveGame();
            if (GUILayout.Button("Load Game")) saveLoadSystem.LoadGame(gameName);
            if (GUILayout.Button("Delete Game")) saveLoadSystem.DeleteGame(gameName);
        }
    }
}