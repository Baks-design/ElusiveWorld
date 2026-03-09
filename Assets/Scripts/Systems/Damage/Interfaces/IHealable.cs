namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IHealable
    {
        float HealthPercentage { get; }

        void Heal(float amount);
    }
}