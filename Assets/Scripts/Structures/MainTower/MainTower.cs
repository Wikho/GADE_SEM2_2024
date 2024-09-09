using UnityEngine;

public class MainTower : MonoBehaviour
{
    private HealthComponent healthComponent;

    private void Awake()
    {
        // Get the HealthComponent attached to this GameObject
        healthComponent = GetComponent<HealthComponent>();

        if (healthComponent == null)
        {
            Debug.LogError("HealthComponent is missing from the MainTower GameObject.");
        }
    }

    private void Update()
    {
        // Check if health is zero or less
        if (healthComponent != null && healthComponent.Health <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! The main tower has been destroyed.");

    }

}
