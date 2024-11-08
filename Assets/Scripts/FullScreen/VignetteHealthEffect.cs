using UnityEngine;

public class VignetteHealthEffect : MonoBehaviour
{
    [Tooltip("Reference to the HealthComponent script")]
    public HealthComponent healthComponent; 

    [Tooltip("Maximum vignette strength when health is at minimum")]
    public float maxVignetteStrength = 1.5f;

    [Tooltip("Material used for the vignette effect")]
    public Material fullscreenMaterial; 

    void Update()
    {
        if (fullscreenMaterial != null && healthComponent != null)
        {
            float health = healthComponent.Health;
            float maxHealth = healthComponent.MaxHealth;

            float normalizedHealth = Mathf.Clamp01(health / maxHealth);

            float vignetteStrength = (1 - normalizedHealth) * maxVignetteStrength;

            fullscreenMaterial.SetFloat("_VignettePower", vignetteStrength);
        }
    }

    private void OnDisable()
    {
        fullscreenMaterial.SetFloat("_VignettePower", 0);
    }

    private void OnDestroy()
    {
        fullscreenMaterial.SetFloat("_VignettePower", 0);
    }
}
