using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Resource Text")]
    [SerializeField] private TMP_Text wood;
    [SerializeField] private TMP_Text stone;

    [Header("Timer Settings")]
    [SerializeField] private TMP_Text timerText;
    private float elapsedTime;
    private bool isTimerRunning = false;
    private bool timerIsPaused = false;

    [Header("Wave Update")]
    [SerializeField] private GameObject startWaveBtn;
    [SerializeField] private GameObject skipWaveBtn;
    [SerializeField] private GameObject waveUpdatePanel;
    [SerializeField] private TMP_Text waveCounterText;
    [SerializeField] private TMP_Text waveUpdateText;

    [Header("Wave Fade Settings")]
    [SerializeField] private float displayDuration = 2.0f;  
    [SerializeField] private float fadeDuration = 1.0f;     
    [SerializeField] private CanvasGroup waveUpdateCanvasGroup; 

    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject pauseMenu;

    [Header("End Game Settings")]
    [SerializeField] private GameObject endGamePanel; 
    [SerializeField] private TMP_Text endGameWaveText; 
    [SerializeField] private TMP_Text endGameTimeText;

    [Header("Resource Warning Settings")]
    [SerializeField] private GameObject notEnoughResourcesPanel;
    [SerializeField] private float displayDurationNER = 1.0f;

    private Coroutine resourceWarningCoroutine;

    [Header("Main Tower Health")]
    [SerializeField] private TMP_Text mainTowerHealth;


    private void Awake()
    {
        Singleton();
        StartTimer();
    }

    private void Update()
    {
        //Timer
        if (isTimerRunning && !timerIsPaused)
        {
            UpdateTimer();
        }

        // Check for "Escape" key press to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    #region Functions

    //Wave
    public void UpdateWave(string value,int waveCount)
    {
        waveCounterText.text = "Wave: " + waveCount.ToString();
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

    #region Pause Menu

    // Function to toggle pause menu
    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            mainUI.SetActive(true);
            ResumeTimer(); 
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            mainUI.SetActive(false);
            PauseTimer(); 
            Time.timeScale = 0f;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the current scene
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Load the Main Menu scene, replace "MainMenu" with your actual main menu scene name
    }

    #endregion

    #region Resources
    public void UpdateUiWood()
    {
        wood.text =": " + ResourceManager.Instance.Wood.ToString();
    }

    public void UpdateUiStone()
    {
        stone.text = ": " + ResourceManager.Instance.Stone.ToString();
    }

    #endregion

    #region Timer Functions
    public void StartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        timerIsPaused = false;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        timerIsPaused = false;
    }

    public void PauseTimer()
    {
        timerIsPaused = true;
    }

    public void ResumeTimer()
    {
        timerIsPaused = false;
    }

    private void UpdateTimer()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    #endregion

    #region Not Enought Resources PopUp

    public void NotEnoughResources()
    {
        // If a warning is already being displayed, stop the previous coroutine
        if (resourceWarningCoroutine != null)
        {
            StopCoroutine(resourceWarningCoroutine);
        }

        // Start the coroutine to show the warning panel
        resourceWarningCoroutine = StartCoroutine(ShowResourceWarning());
    }

    private IEnumerator ShowResourceWarning()
    {
        // Enable the warning panel
        notEnoughResourcesPanel.SetActive(true);

        // Wait for the specified display duration
        yield return new WaitForSeconds(displayDurationNER);

        // Disable the warning panel after the duration
        notEnoughResourcesPanel.SetActive(false);

        // Clear the coroutine reference
        resourceWarningCoroutine = null;
    }


    #endregion

    #region GameOver

    public void GameOver()
    {
        //Pause the game
        Time.timeScale = 0f;

        //Hide other UI elements
        mainUI.SetActive(false);
        pauseMenu.SetActive(false);
        waveUpdatePanel.SetActive(false);

        //Display the end game panel
        endGamePanel.SetActive(true);

        //Update the wave and time texts
        endGameWaveText.text = "Waves Survived: " + EnemySpawnSettings.Instance.GetWave().ToString();
        endGameTimeText.text = "Time Survived: " + timerText.text;
    }


    #endregion

    #region

    public void MainTowerHealthUI(float currentHealth, float maxHealth)
    {
        int healthPercentage = Mathf.Clamp(Mathf.RoundToInt((currentHealth / maxHealth) * 100), 0, 100);

        mainTowerHealth.text =": " + healthPercentage.ToString();
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
    }
    #endregion
}
