using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSettings : MonoBehaviour
{
    #region Variables
    public static EnemySpawnSettings Instance;

    [Header("Waves")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int firstWaveEnemySize = 10;
    [SerializeField][Range(1f, 2f)] private float nextWaveEnemyMultiplier = 1.3f;
    [SerializeField][Range(1, 10)] private int nextWaveEnemyAddRandomMin = 2;
    [SerializeField][Range(1, 10)] private int nextWaveEnemyAddRandomMax = 5;

    [Header("Enemy Prefabs")]
    public GameObject goblinPrefab;
    public GameObject wolfPrefab;
    public GameObject golemPrefab;

    [Header("Enemy Spawn Ratios")]
    [SerializeField][Range(0f, 1f)] private float goblinSpawnRatio = 1f; // Starts at 100%
    [SerializeField][Range(0f, 1f)] private float wolfSpawnRatio = 0f;
    [SerializeField][Range(0f, 1f)] private float golemSpawnRatio = 0f;

    [Header("Wave Milestones")]
    [SerializeField] private int wolfUnlockWave = 6;
    [SerializeField] private int golemUnlockWave = 10;
    [SerializeField] private int bossWaveInterval = 20; // Boss wave

    [Header("Boss Wave Settings")]
    [SerializeField] private float bossWaveEnemyCountMultiplier = 1.5f;
    [SerializeField][Range(1f, 3f)] private float bossWaveWolfGolemRatioMultiplier = 2f;

    [System.Serializable]
    public class EnemyAttributeMultipliers
    {
        public float healthMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float damageMultiplier = 1f;
    }

    public EnemyAttributeMultipliers goblinMultipliers = new EnemyAttributeMultipliers();
    public EnemyAttributeMultipliers wolfMultipliers = new EnemyAttributeMultipliers();
    public EnemyAttributeMultipliers golemMultipliers = new EnemyAttributeMultipliers();

    [Header("Player Stats")]
    [SerializeField] private int woodValue = 0;
    [SerializeField] private int stoneValue = 0;
    [SerializeField] private int totalBuildingsBuilt = 0;

    [Header("Wave Stats")]
    [SerializeField] private int enemiesAlive;

    [Header("Round Settings")]
    [SerializeField] private float timeBetweenRounds = 10f;
    private Coroutine roundTimerCoroutine;

    private Transform enemySpawnParent;
    [SerializeField] private List<EnemySpawnPoint> enemySpawnPoints = new List<EnemySpawnPoint>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        GetEnemySpawnPoints();
    }

    private void Update()
    {
        EnemiesLeft();

        // Update player stats from other managers if needed
        woodValue = ResourceManager.Instance.Wood;
        stoneValue = ResourceManager.Instance.Stone;
        totalBuildingsBuilt = ClickToSpawnManager.instance.totalBuildingsBuild;
    }
    #endregion

    #region Functions

    private void GetEnemySpawnPoints()
    {
        enemySpawnPoints.Clear();

        enemySpawnParent = GameObject.FindGameObjectWithTag("Enemies").transform;

        GameObject[] spawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
        foreach (GameObject spawner in spawners)
        {
            EnemySpawnPoint spawnPoint = spawner.GetComponent<EnemySpawnPoint>();
            if (spawnPoint != null)
            {
                enemySpawnPoints.Add(spawnPoint);
            }
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

        UpdateEnemySpawnRatios();

        int waveSize = CalculateWaveSize();
        Dictionary<GameObject, int> enemiesToSpawn = DetermineEnemiesToSpawn(waveSize);

        foreach (EnemySpawnPoint sp in enemySpawnPoints)
        {
            sp.StartSpawning(enemiesToSpawn, enemySpawnParent);
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
            UiManager.Instance.UpdateWave($"Wave {currentWave}\nThe enemies got stronger!", currentWave);
        }
        else if(currentWave % bossWaveInterval == 0)
        {
            UiManager.Instance.UpdateWave($"Wave {currentWave}\nBoss wave started.\nThe enemies got stronger.", currentWave);
        }
        else
        {
            UiManager.Instance.UpdateWave($"Wave {currentWave}", currentWave);
        }
    }

    private int CalculateWaveSize()
    {
        // Calculate the base size of the wave using the multiplier
        int baseWaveSize = Mathf.RoundToInt(firstWaveEnemySize * Mathf.Pow(nextWaveEnemyMultiplier, currentWave - 1));

        // Add a random amount to the wave size
        int randomAddition = Random.Range(nextWaveEnemyAddRandomMin, nextWaveEnemyAddRandomMax + 1);

        int waveSize = baseWaveSize + randomAddition;

        // Adjust for boss waves
        if (currentWave % bossWaveInterval == 0)
        {
            waveSize = Mathf.RoundToInt(waveSize * bossWaveEnemyCountMultiplier);
        }

        return waveSize;
    }

    private void IncreaseEnemyMultipliers()
    {
        goblinMultipliers.healthMultiplier *= 1.1f;
        goblinMultipliers.speedMultiplier *= 1.05f;
        goblinMultipliers.damageMultiplier *= 1.1f;

        wolfMultipliers.healthMultiplier *= 1.1f;
        wolfMultipliers.speedMultiplier *= 1.05f;
        wolfMultipliers.damageMultiplier *= 1.1f;

        golemMultipliers.healthMultiplier *= 1.1f;
        golemMultipliers.speedMultiplier *= 1.05f;
        golemMultipliers.damageMultiplier *= 1.1f;
    }

    private void UpdateEnemySpawnRatios()
    {
        // Reset ratios
        goblinSpawnRatio = 1f;
        wolfSpawnRatio = 0f;
        golemSpawnRatio = 0f;

        // Unlock wolves after wolfUnlockWave
        if (currentWave >= wolfUnlockWave)
        {
            wolfSpawnRatio = 0.15f; // 15% chance
            goblinSpawnRatio -= wolfSpawnRatio;
        }

        // Unlock golems after golemUnlockWave
        if (currentWave >= golemUnlockWave)
        {
            golemSpawnRatio = 0.1f; // 10% chance
            goblinSpawnRatio -= golemSpawnRatio * 0.5f;
            wolfSpawnRatio -= golemSpawnRatio * 0.5f;
        }

        // Adjust ratios for boss waves
        if (currentWave % bossWaveInterval == 0)
        {
            wolfSpawnRatio *= bossWaveWolfGolemRatioMultiplier;
            golemSpawnRatio *= bossWaveWolfGolemRatioMultiplier;
            goblinSpawnRatio = 1f - (wolfSpawnRatio + golemSpawnRatio);
        }
    }

    private Dictionary<GameObject, int> DetermineEnemiesToSpawn(int waveSize)
    {
        Dictionary<GameObject, int> enemiesToSpawn = new Dictionary<GameObject, int>();

        int goblinCount = Mathf.RoundToInt(waveSize * goblinSpawnRatio);
        int wolfCount = Mathf.RoundToInt(waveSize * wolfSpawnRatio);
        int golemCount = Mathf.RoundToInt(waveSize * golemSpawnRatio);

        // Ensure total enemy count matches waveSize
        int totalAssigned = goblinCount + wolfCount + golemCount;
        int difference = waveSize - totalAssigned;
        if (difference > 0)
        {
            goblinCount += difference; // Assign the remaining to goblins
        }

        enemiesToSpawn.Add(goblinPrefab, goblinCount);
        if (wolfCount > 0) enemiesToSpawn.Add(wolfPrefab, wolfCount);
        if (golemCount > 0) enemiesToSpawn.Add(golemPrefab, golemCount);

        return enemiesToSpawn;
    }

    private void EnemiesLeft()
    {
        enemiesAlive = enemySpawnParent.childCount;
    }

    public void EndWave()
    {
        // Destroy all enemy game objects
        foreach (Transform enemy in enemySpawnParent)
        {
            Destroy(enemy.gameObject);
        }

        // Stop current spawning coroutines in enemy spawn points
        foreach (EnemySpawnPoint sp in enemySpawnPoints)
        {
            sp.StopSpawning();
        }

        // Immediately start the next round
        if (roundTimerCoroutine != null)
        {
            StopCoroutine(roundTimerCoroutine);
        }
        StartCoroutine(CheckForEndOfRound());
    }

    public void GameOver()
    {
        // Stop all spawning
        foreach (EnemySpawnPoint sp in enemySpawnPoints)
        {
            sp.StopAllCoroutines();
        }

        // Destroy all enemies
        foreach (Transform enemy in enemySpawnParent)
        {
            Destroy(enemy.gameObject);
        }

        // Notify UI Manager
        UiManager.Instance.GameOver();

    }

    #endregion

    #region Public Methods

    public void StartWave()
    {
        StartNextRound();
    }

    public void SkipWave()
    {
        SkipRoundTime();
    }

    public int GetWave()
    {
        return currentWave;
    }

    public void UpdateEnemySpawners()
    {
        GetEnemySpawnPoints();
    }

    #endregion

    #region Singleton
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of EnemySpawnSettings was destroyed!");
            return;
        }

        Instance = this;
    }
    #endregion
}
