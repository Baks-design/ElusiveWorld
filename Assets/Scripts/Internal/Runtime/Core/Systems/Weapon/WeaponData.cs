using UnityEngine;

namespace VHS
{
    public enum WeaponTriggerType { PullRelease, Continous }

    [CreateAssetMenu(menuName = "Systems/Weapon/weaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] WeaponTriggerType triggerType;
        [SerializeField, Tooltip("-1 = infinite")] int ammoCount = 0;
        [SerializeField] int roundsPerSecond = 0;
        [SerializeField] float reloadDuration = 0f;
        float timeBetweenRounds;

        public WeaponTriggerType TriggerType => triggerType;
        public int AmmoCount => ammoCount;
        public int RoundsPerSecond => roundsPerSecond;
        public float ReloadDuration => reloadDuration;
        public float TimeBetweenRounds => timeBetweenRounds;

        public void Init() => timeBetweenRounds = 1f / roundsPerSecond;
    }
}
