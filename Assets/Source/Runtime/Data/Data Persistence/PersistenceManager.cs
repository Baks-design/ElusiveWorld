using System.Collections.Generic;
using System.Linq;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Entities;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence
{
    public class PersistenceManager : MonoBehaviour, IService
    {
        IDataService dataService;

        [field: SerializeField] public GameData GameData { get; set; }

        public void Initialize()
        {
            dataService = new FileDataService(new JsonSerializer());
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Dispose() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu") return;
            Bind<PlayerEntity, PlayerData>(GameData.playerData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>().FirstOrDefault();
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>();
            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            GameData = new GameData
            {
                Name = "GameTest",
                CurrentLevelName = "PlayerTest"
            };
            SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void SaveGame() => dataService.Save(GameData);

        public void LoadGame(string gameName)
        {
            GameData = dataService.Load(gameName);

            if (string.IsNullOrWhiteSpace(GameData.CurrentLevelName))
                GameData.CurrentLevelName = "PlayerTest";

            SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(GameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}