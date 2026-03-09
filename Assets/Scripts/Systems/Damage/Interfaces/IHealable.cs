namespace ElusiveWorld.Core.Assets.Scripts.Systems.Damage.Interfaces
{
    public interface IHealable
    {
        float HealthPercentage { get; }

        void Heal(float amount);
    }
}