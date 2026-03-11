using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}