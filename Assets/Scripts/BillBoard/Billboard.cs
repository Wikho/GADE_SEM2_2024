using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]private float maxUpwardAngle = 45f;
    [SerializeField] private Image healthboard;

    //private
    private Camera mainCamera;
    private HealthComponent healthComponent;
    private EnemyController enemyController;

    #region Unity Methods
    void Start()
    {
        mainCamera = Camera.main;
        healthComponent = GetComponent<HealthComponent>();
        enemyController = GetComponent<EnemyController>();
    }

    void LateUpdate()
    {
        if (healthboard != null)
        {
            // Make the healthboard face the camera
            Vector3 directionToCamera = mainCamera.transform.position - healthboard.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // Get the rotation angles
            Vector3 targetEulerAngles = targetRotation.eulerAngles;

            // Clamp the upward angle to the maxUpwardAngle
            if (targetEulerAngles.x > 180f)
            {
                targetEulerAngles.x -= 360f;
            }
            targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x, -maxUpwardAngle, maxUpwardAngle);

            // Apply the rotation only to the healthboard
            healthboard.transform.rotation = Quaternion.Euler(targetEulerAngles.x, targetEulerAngles.y, 0f);
        }

        // Update the health bar fill amount
        
        //For Towers
        if (healthComponent != null && healthboard != null)
        {
            // Calculate the fill amount as a float between 0 and 1
            float fillAmount = Mathf.Clamp((float)healthComponent.Health / healthComponent.MaxHealth, 0f, 1f);
            healthboard.fillAmount = fillAmount;
        }

        //For Enemy
        if (enemyController != null && healthboard != null)
        {
            // Calculate the fill amount as a float between 0 and 1
            float fillAmount = Mathf.Clamp(enemyController.Health() / enemyController.MaxHealth(), 0f, 1f);
            healthboard.fillAmount = fillAmount;
        }
    }
    #endregion
}
