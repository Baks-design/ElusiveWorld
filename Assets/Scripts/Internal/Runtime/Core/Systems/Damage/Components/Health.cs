using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Components
{
    public class Health : MonoBehaviour
    {
        [field: SerializeField] public int MaxLives { get; private set; } = 3;
        [field: SerializeField] public float MaxHealth { get; } = 100f;
        public float CurrentHealth { get; private set; }
        public int CurrentLives { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;

        void Start()
        {
            CurrentHealth = MaxHealth;
            CurrentLives = MaxLives;
        }

        public void ModifyHealth(float amount) =>
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);

        public void DecreaseLife()
        {
            if (CurrentLives > 0)
                CurrentLives--;
        }

        public void ResetHealth() => CurrentHealth = MaxHealth;
    }
}