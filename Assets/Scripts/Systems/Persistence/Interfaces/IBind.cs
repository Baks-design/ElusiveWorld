using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        
        void Bind(TData data);
    }
}