using UnityEngine;

public class UiPulseEffect : MonoBehaviour
{
    public float pulseSpeed = 1.0f; // Speed of the pulsing effect
    public float pulseAmount = 0.1f; // Amount to scale up and down

    private Vector3 initialScale; // Original scale of the object

    void Start()
    {
        // Store the initial scale of the object
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Calculate the pulsing scale
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        // Apply the pulsing scale to the object's transform
        transform.localScale = initialScale * scale;
    }
}
