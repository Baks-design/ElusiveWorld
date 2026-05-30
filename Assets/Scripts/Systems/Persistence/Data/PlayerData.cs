using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Helpers;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Persistence.Data
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;

        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}