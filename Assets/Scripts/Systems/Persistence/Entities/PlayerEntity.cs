using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Entities
{
    public class PlayerEntity : MonoBehaviour, IBind<PlayerData>
    {
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data) => data.Id = Id;
    }
}