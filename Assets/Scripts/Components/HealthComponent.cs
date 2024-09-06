using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made to be very flexible, so don't worry about the items with 0 references, that's because they'll be useful down the line.
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float currentHealth = 0;
    [SerializeField] private float maxHealth = 0;

    public bool IsDead { get => currentHealth <= 0; }
    public float Health { get => currentHealth; }
    public float MaxHealth { get => maxHealth; }
    public float HealthNormalized { get => currentHealth / maxHealth; }

    private void Start()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (IsDead)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
