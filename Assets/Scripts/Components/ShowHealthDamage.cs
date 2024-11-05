using UnityEngine;

public class ShowHealthDamage : MonoBehaviour
{
    public Renderer _renderer;
    private MaterialPropertyBlock propertyBlock;
    public HealthComponent healthComponent;

    // The name of the ShaderGraph property controlling texture blending
    private static readonly int TextureBlendingAmountID = Shader.PropertyToID("_TextureBlendingAmount");

    void Start()
    {
        // Initialize the MaterialPropertyBlock
        propertyBlock = new MaterialPropertyBlock();

        if(healthComponent == null)
        {
            try
            {
                // Try to get the HealthComponent
                healthComponent = GetComponent<HealthComponent>();
            }
            catch
            {

            }
        }

    }

    void Update()
    {
        // Check if HealthComponent is null and exit if it is
        if (healthComponent == null) return;

        // Get the health value and convert it to a 0 to 1 range, inverted
        float healthNormalized = Mathf.Clamp01(healthComponent.Health / 100f);
        float invertedHealthNormalized = 1 - healthNormalized;

        // Set the shader's _TextureBlendingAmount to the inverted health value
        propertyBlock.SetFloat(TextureBlendingAmountID, invertedHealthNormalized);

        // Apply the MaterialPropertyBlock to this specific renderer
        _renderer.SetPropertyBlock(propertyBlock);
    }
}
