using UnityEngine;

public class ShowHealthDamage : MonoBehaviour
{
    private Renderer _renderer;
    public Material material;
    private HealthComponent healthComponent;

    // The name of the ShaderGraph property controlling texture blending
    private static readonly int TextureBlendingAmountID = Shader.PropertyToID("_TextureBlendingAmount");

    void Start()
    {
        if (_renderer == null)
        {
            Debug.Log("Need material assigned at script.");
        }

        // Try to get the HealthComponent
        healthComponent = GetComponent<HealthComponent>();
    }

    void Update()
    {
        // Check if HealthComponent is null and exit if it is
        if (healthComponent == null || material == null) return;

        // Get the health value and convert it to a 0 to 1 range
        float healthNormalized = Mathf.Clamp01(healthComponent.Health / healthComponent.MaxHealth); 

         // Invert the health normalized value so 1 is damage, and 0 is full health
        float invertedHealthNormalized = 1 - healthNormalized;


        // Set the shader's _TextureBlendingAmount to the normalized health value
        material.SetFloat(TextureBlendingAmountID, invertedHealthNormalized);
    }
}
