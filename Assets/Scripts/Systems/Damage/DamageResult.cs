using System;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage
{
    [Serializable]
    public class DamageResult
    {
        public float rawDamage;
        public float finalDamage;
        public float mitigation;
        public bool isBlocked;
        public bool isDodged;
        public bool isCritical;
        public DamageType damageType;

        public override string ToString()
            => $"Damage: {finalDamage:F0} (Raw: {rawDamage:F0}, Mitigation: {mitigation:P0})";
    }
}