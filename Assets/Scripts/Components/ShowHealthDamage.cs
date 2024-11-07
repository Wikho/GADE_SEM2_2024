using UnityEngine;

public class ShowHealthDamage : MonoBehaviour
{
    public Renderer _renderer;
    private MaterialPropertyBlock propertyBlock;
    public HealthComponent healthComponent;

    private static readonly int TextureBlendingAmountID = Shader.PropertyToID("_TextureBlendingAmount");

    void Start()
    {
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

    void FixedUpdate()
    {
        if (healthComponent == null) return;

        float healthNormalized = Mathf.Clamp01(healthComponent.Health / healthComponent.MaxHealth);
        float invertedHealthNormalized = 1 - healthNormalized;

        propertyBlock.SetFloat(TextureBlendingAmountID, invertedHealthNormalized);

        _renderer.SetPropertyBlock(propertyBlock);
    }
}
