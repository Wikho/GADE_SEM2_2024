using UnityEngine;

public class MainTower : MonoBehaviour
{
    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();

        if (healthComponent == null)
        {
            Debug.LogError("HealthComponent is missing from the MainTower GameObject.");
        }
    }

    private void FixedUpdate()
    {
        if (healthComponent != null && healthComponent.Health <= 0)
        {
            GameOver();
        }
        else
        {
            UiManager.Instance.MainTowerHealthUI(healthComponent.Health, healthComponent.MaxHealth);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! The main tower has been destroyed.");
        UiManager.Instance.GameOver();
    }

}
