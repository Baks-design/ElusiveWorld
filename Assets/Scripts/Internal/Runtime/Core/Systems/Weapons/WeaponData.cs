using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public enum WeaponTriggerType { PullRelease, Continous }

    [CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Systems/Weapons/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [field: SerializeField] public WeaponTriggerType TriggerType { get; private set; } = WeaponTriggerType.PullRelease;
        [field: SerializeField, Tooltip("-1 = infinite")] public int AmmoCount { get; private set; } = 0;
        [field: SerializeField] public int RoundsPerSecond { get; private set; } = 0;
        [field: SerializeField] public float ReloadDuration { get; private set; } = 0f;
        public float TimeBetweenRounds { get; private set; }

        public void Init() => TimeBetweenRounds = 1f / RoundsPerSecond;
    }
}
