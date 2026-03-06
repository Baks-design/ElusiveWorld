namespace Assets.Scripts.Internal.Runtime.Core.Systems.Damage.Interfaces
{
    public interface IDamageCalculator
    {
        float CalculateDamage(float baseDamage, float modifiers);
    }
}