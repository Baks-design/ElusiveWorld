using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
    }
}