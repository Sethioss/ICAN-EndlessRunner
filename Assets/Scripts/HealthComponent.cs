using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int Health;
    [SerializeField] private int MaxHealth;

    private void Start()
    {
        Health = MaxHealth;
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public void RemoveHealth()
    {
        Health--;
    }

    public void AddHealth()
    {
        Health++;
    }

    public void FullyHeal()
    {
        Health = MaxHealth;
    }
}
