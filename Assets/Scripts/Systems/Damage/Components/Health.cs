using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Components
{
    public class Health : MonoBehaviour //TODO: Adjust Health Function
    {
        [field: SerializeField, Range(1, 3)] public int MaxLives { get; private set; } = 3;
        [field: SerializeField, Range(10f, 100f)] public float MaxHealth { get; private set; } = 100f;
        public bool IsAlive => CurrentHealth > 0f;
        public float CurrentHealth { get; private set; }
        public int CurrentLives { get; private set; }

        void Start()
        {
            CurrentHealth = MaxHealth;
            CurrentLives = MaxLives;
        }

        public void ModifyHealth(float amount) =>
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);

        public void DecreaseHealth()
        {
            if (CurrentLives > 0)
                CurrentLives--;
        }

        public void ResetHealth() => CurrentHealth = MaxHealth;
    }
}