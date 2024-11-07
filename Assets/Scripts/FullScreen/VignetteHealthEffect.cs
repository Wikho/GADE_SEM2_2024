using UnityEngine;

public class VignetteHealthEffect : MonoBehaviour
{
    [Tooltip("Reference to the HealthComponent script")]
    public HealthComponent healthComponent; // Assign your health component here

    [Tooltip("Maximum vignette strength when health is at minimum")]
    public float maxVignetteStrength = 1.5f;

    [Tooltip("Material used for the vignette effect")]
    public Material fullscreenMaterial; // Assign the material directly in the Inspector

    void Update()
    {
        if (fullscreenMaterial != null && healthComponent != null)
        {
            float health = healthComponent.Health;
            float maxHealth = healthComponent.MaxHealth;

            // Normalize health (0 to 1)
            float normalizedHealth = Mathf.Clamp01(health / maxHealth);

            // Invert and scale to get vignette strength (0 to maxVignetteStrength)
            float vignetteStrength = (1 - normalizedHealth) * maxVignetteStrength;

            // Update the shader parameter
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
