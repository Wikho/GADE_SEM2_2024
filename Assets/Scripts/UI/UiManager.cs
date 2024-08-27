using System.Collections;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Wave Update")]
    [SerializeField] private GameObject startWaveBtn;
    [SerializeField] private GameObject skipWaveBtn;
    [SerializeField] private GameObject waveUpdatePanel;
    [SerializeField] private TMP_Text waveUpdateText;

    [Header("Wave Fade Settings")]
    [SerializeField] private float displayDuration = 2.0f;  // Time to display the panel before fading
    [SerializeField] private float fadeDuration = 1.0f;     // Duration of the fade-out
    [SerializeField] private CanvasGroup waveUpdateCanvasGroup; // CanvasGroup for fading

    private void Awake()
    {
        Singleton();
    }

    #region Functions

    //Wave
    public void UpdateWave(string value)
    {
        waveUpdateText.text = value;
        waveUpdatePanel.SetActive(true);
        waveUpdateCanvasGroup.alpha = 1.0f; // Make sure it's fully visible

        // Start the process to fade out the panel after displaying it
        StartCoroutine(FadeOutWaveUpdatePanel());
    }

    private IEnumerator FadeOutWaveUpdatePanel()
    {
        // Wait for the display duration before starting to fade out
        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            waveUpdateCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
            yield return null;
        }

        waveUpdateCanvasGroup.alpha = 0.0f; // Ensure it’s completely invisible
        waveUpdatePanel.SetActive(false);   // Optionally, deactivate the panel
    }

    public void StartWave()
    {
        EnemySpawnSettings.Instance.StartWave();
        startWaveBtn.SetActive(false);
    }

    public void SkipWave()
    {
        EnemySpawnSettings.Instance.SkipWave();
        startWaveBtn.SetActive(false);
    }

    #endregion

    #region GetSet

    public void EnableSkipWave()
    {
        skipWaveBtn.SetActive(true);
    }
    public void DiableSkipWave()
    {
        skipWaveBtn.SetActive(false);
    }

    #endregion

    #region Singleton
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of UiManager was destroyed on creation!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
