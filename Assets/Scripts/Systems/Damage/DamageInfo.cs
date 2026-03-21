using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    [Serializable]
    public struct DamageInfo
    {
        public float baseDamage;
        public DamageType damageType;
        public GameObject source;
        public GameObject target;
        public bool isCritical;
        public bool ignoresResistances;
        public Vector3 hitPoint;
        public Vector3 hitDirection;

        public DamageInfo(float damage, DamageType type, GameObject source = null)
        {
            baseDamage = damage;
            damageType = type;
            this.source = source;

            target = null;
            isCritical = false;
            ignoresResistances = false;
            hitPoint = Vector3.zero;
            hitDirection = Vector3.zero;
        }
    }
}