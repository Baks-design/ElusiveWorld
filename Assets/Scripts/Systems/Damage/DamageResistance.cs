using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    [Serializable]
    public class DamageResistance
    {
        public DamageType type;
        [Range(0f, 1f)] public float reduction = 0f;
        [Range(0f, 1f)] public float immunity = 0f;
    }
}