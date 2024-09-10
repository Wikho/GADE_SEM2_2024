using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawnSettings : MonoBehaviour
{
    #region Variables
    public static EnemySpawnSettings Instance;

    private bool isAvtive = false;

    [Header("Waves")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int firstWaveEnemySize = 10;
    [SerializeField][Range(1f, 2f)] private float nextWaveEnemyMultiplier = 1.3f;
    [SerializeField][Range(1, 10)] int nextWaveEnemyAddRandomMin = 2;
    [SerializeField][Range(1, 10)] int nextWaveEnemyAddRandomMax = 5;

    [Space]
    [Header("Enemy Attributes")]
    [SerializeField] private float enemyHealthMultiplier = 1.0f;
    [SerializeField] private float enemySpeedMultiplier = 1.0f;
    [SerializeField] private float enemyDamageMultiplier = 1.0f;

    [Space]
    [Header("Wave Stats")]
    [SerializeField] private int enemiesAlive;

    [Header("Round Settings")]
    [SerializeField] private float timeBetweenRounds = 10f; 
    private Coroutine roundTimerCoroutine;

    //private 
    private Transform enemySpawnParent;
    [SerializeField] private List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();

    #endregion

    #region Unity Methods
    private void Awake()
    {
        Singleton();
    }

    private void FixedUpdate()
    {
        if (isAvtive)
        {
            isAvtive = false;
            StartNextRound();
        }

        EnemiesLeft();
    }

    #endregion

    #region Function

    private void GetTowersAndEnemies()
    {
        //Clear list if regenrate
        enemySpawnPoints.Clear();

        //Find Place where all enemies will spawn
        enemySpawnParent = GameObject.FindGameObjectWithTag("Enemies").transform;

        while (TerrainGenerator.Instance == null)
        {
            Task.Delay(100).Wait();
        }

        // Find all enemy spawn points by tag and get the script component
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
        foreach (GameObject spawner in spawners)
        {
            EnemySpawnPoint spawnPoint = spawner.GetComponent<EnemySpawnPoint>();
            if (spawnPoint != null)
            {
                enemySpawnPoints.Add(spawnPoint);
                //Debug.Log("Added Spawn Points");
            }
        }

        // Ensure that the number of spawn points matches the expected amount
        int expectedSpawnPoints = TerrainGenerator.Instance.GetNumberOfSpawnPoints();
        if (enemySpawnPoints.Count != expectedSpawnPoints)
        {
            Debug.LogError($"Mismatch in spawn points. Expected: {expectedSpawnPoints}, Found: {enemySpawnPoints.Count}");
        }
    }

    private void StartNextRound()
    {
        UiManager.Instance.DiableSkipWave();

        currentWave++;

        SendUiWaveUpdate();

        // Check if we need to increase multipliers every 5 waves
        if (currentWave % 5 == 0)
        {
            IncreaseEnemyMultipliers();
        }

        int size = WaveSize();
        Debug.Log(size.ToString());

        foreach (EnemySpawnPoint sp in enemySpawnPoints)
        {
            sp.StartRound(size);
        }

        // Start checking if the round is over
        StartCoroutine(CheckForEndOfRound());
    }

    private IEnumerator CheckForEndOfRound()
    {
        // Wait until all enemies are dead
        while (enemySpawnParent.childCount > 0)
        {
            yield return new WaitForSeconds(1f); 
        }

        // Start timer for the next round
        if (roundTimerCoroutine != null)
        {
            StopCoroutine(roundTimerCoroutine);
        }
        roundTimerCoroutine = StartCoroutine(StartRoundTimer());
    }

    private IEnumerator StartRoundTimer()
    {
        UiManager.Instance.EnableSkipWave();
         
        float timer = timeBetweenRounds;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;

        }

        StartNextRound();
    }

    public void SkipRoundTime()
    {
        if (roundTimerCoroutine != null)
        {
            StopCoroutine(roundTimerCoroutine);
            StartNextRound();
        }
    }

    private void SendUiWaveUpdate()
    {
        if (currentWave % 5 == 0)
        {
            UiManager.Instance.UpdateWave($"Wave {currentWave}\nThe enemies got Stronger!",currentWave);
        }
        else
        {
            UiManager.Instance.UpdateWave($"Wave {currentWave}", currentWave);
        }
    }

    private int WaveSize()
    {
        // Calculate the base size of the wave using the multiplier
        int baseWaveSize = Mathf.RoundToInt(firstWaveEnemySize * Mathf.Pow(nextWaveEnemyMultiplier, currentWave));

        // Add a random amount to the wave size
        int randomAddition = Random.Range(nextWaveEnemyAddRandomMin, nextWaveEnemyAddRandomMax + 1);

        // Return the calculated wave size
        return baseWaveSize + randomAddition;
    }

    private void IncreaseEnemyMultipliers()
    {
        enemyHealthMultiplier *= 1.1f;  // Increase health multiplier by 10%
        enemySpeedMultiplier *= 1.0025f;   // Increase speed multiplier by 2,5%
        enemyDamageMultiplier *= 1.1f;  // Increase damage multiplier by 10%
    }

    private void EnemiesLeft()
    {
        enemiesAlive = enemySpawnParent.childCount;
    }

    #endregion

    #region GetSet

    public void UpdateEnemySpawners()
    {
        GetTowersAndEnemies();
    }

    //Begin Wave
    public void StartWave()
    {
        StartNextRound();
    }
    //Skip Wave
    public void SkipWave()
    {
        SkipRoundTime();
    }

    public int GetWave()
    {
        return currentWave;
    }

    #endregion

    #region Singleton
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of EnemySpawnSettings was destroyed on creation!");
            return;
        }

        Instance = this;
    }
    #endregion
}
