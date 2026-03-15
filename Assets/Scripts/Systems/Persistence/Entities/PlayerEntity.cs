using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Types;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Entities
{
    public class PlayerEntity : MonoBehaviour, IEarlyUpdate, IBind<PlayerData>
    {
        [SerializeField] PlayerData data;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            data.Id = Id;
            transform.SetPositionAndRotation(data.position, data.rotation);
        }

        void OnEnable() => UpdateManager.RegisterEarlyUpdate(this);

        void IEarlyUpdate.EarlyUpdate()
        {
            data.position = transform.position;
            data.rotation = transform.rotation;
        }

        void OnDisable() => UpdateManager.UnregisterEarlyUpdate(this);
    }
}