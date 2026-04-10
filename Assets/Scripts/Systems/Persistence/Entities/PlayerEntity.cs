using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Entities
{
    public class PlayerEntity : MonoBehaviour, IUpdate, IBind<PlayerData>
    {
        [SerializeField] PlayerData data;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            data.Id = Id;
            transform.SetPositionAndRotation(data.position, data.rotation);
        }

        void OnEnable() => UpdateManager.RegisterUpdate(this);

        void IUpdate.Update(float dt)
        {
            data.position = transform.position;
            data.rotation = transform.rotation;
        }

        void OnDisable() => UpdateManager.UnregisterUpdate(this);
    }
}