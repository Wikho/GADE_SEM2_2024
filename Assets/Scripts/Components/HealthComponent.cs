using System.Collections;
using UnityEngine;

// Made to be very flexible, so don't worry about the items with 0 references, that's because they'll be useful down the line.
public class HealthComponent : MonoBehaviour
{
    #region Variable
    public float currentHealth = 0;
    public float maxHealth = 0;

    [Header("Health Regeneration Settings")]
    [SerializeField] private bool canRegen = true; //Can regenerate health
    [SerializeField] private float regenRate = 1f; //Amount to regenerate per second
    [SerializeField] private float regenAmount = 5f; //Total amount of health to regenerate
    [SerializeField] private float regenDelay = 5f; //Delay before starting regeneration

    [SerializeField] private bool isRegenerating = false;
    [SerializeField] private float timeSinceLastDamage = 0f; 
    private Coroutine regenCoroutine;

    #endregion

    #region GetSet
    public bool IsDead { get => currentHealth <= 0; }
    public float Health { get => currentHealth; }
    public float MaxHealth { get => maxHealth; }
    public float HealthNormalized { get => currentHealth / maxHealth; }

    #endregion

    private void Start()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (damage > 0)
        {
            timeSinceLastDamage = 0f;

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
                isRegenerating = false;
            }
        }

        if (currentHealth < 0)
            currentHealth = 0;

        if (IsDead)
            Die();
    }

    public void Die()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine); 
        }
        Destroy(gameObject,1f);
    }

    #region Regen Health

    private void FixedUpdate()
    {
        if (canRegen && !IsDead)
        {
            timeSinceLastDamage += Time.fixedDeltaTime;

            if (!isRegenerating && timeSinceLastDamage >= regenDelay)
            {
                regenCoroutine = StartCoroutine(RegenerateHealth());
            }
        }
    }

    private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;

        float totalRegened = 0f;
        while (totalRegened < regenAmount && currentHealth < maxHealth)
        {
            float regenThisFrame = regenRate * Time.deltaTime;
            currentHealth += regenThisFrame;
            totalRegened += regenThisFrame;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            yield return null;
        }

        isRegenerating = false;
        regenCoroutine = null; 
    }

    #endregion
}
