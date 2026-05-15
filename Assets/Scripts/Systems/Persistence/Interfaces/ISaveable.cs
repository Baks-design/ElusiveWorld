using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}