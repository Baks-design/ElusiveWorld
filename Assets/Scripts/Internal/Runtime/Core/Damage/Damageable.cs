using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable, IRecoverable
{
    public void IncreaseHealth(int amount) { }

    public void ReduceHealth(int amount) { }
}