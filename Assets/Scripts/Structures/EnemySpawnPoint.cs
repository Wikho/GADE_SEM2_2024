using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    #region Variables

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;
    private Transform enemySpawnParent;
    private Coroutine spawningCoroutine;

    #endregion

    private void Awake()
    {
        // Find the parent object for enemies
        enemySpawnParent = GameObject.FindGameObjectWithTag("Enemies").transform;
    }

    public void StartSpawning(Dictionary<GameObject, int> enemiesToSpawn, Transform parent)
    {
        StartCoroutine(SpawnEnemies(enemiesToSpawn, parent));
    }

    public void StopSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemies(Dictionary<GameObject, int> enemiesToSpawn, Transform parent)
    {
        List<GameObject> enemyList = new List<GameObject>();

        foreach (var kvp in enemiesToSpawn)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                enemyList.Add(kvp.Key);
            }
        }

        // Shuffle the list to randomize spawn order
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject temp = enemyList[i];
            int randomIndex = Random.Range(i, enemyList.Count);
            enemyList[i] = enemyList[randomIndex];
            enemyList[randomIndex] = temp;
        }

        foreach (GameObject enemyPrefab in enemyList)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, parent);

            // Set enemy attributes here
            EnemyController enemyScript = enemy.GetComponent<EnemyController>();

            if (enemyScript != null)
            {
                // Adjust attributes based on the multipliers
                if (enemyPrefab == EnemySpawnSettings.Instance.goblinPrefab)
                {
                    enemyScript.healthMultiplier(EnemySpawnSettings.Instance.goblinMultipliers.healthMultiplier);
                    enemyScript.speedMultiplier(EnemySpawnSettings.Instance.goblinMultipliers.speedMultiplier);
                    enemyScript.damageMultiplier(EnemySpawnSettings.Instance.goblinMultipliers.damageMultiplier);
                }
                else if (enemyPrefab == EnemySpawnSettings.Instance.wolfPrefab)
                {
                    enemyScript.healthMultiplier(EnemySpawnSettings.Instance.wolfMultipliers.healthMultiplier);
                    enemyScript.speedMultiplier(EnemySpawnSettings.Instance.wolfMultipliers.speedMultiplier);
                    enemyScript.damageMultiplier(EnemySpawnSettings.Instance.wolfMultipliers.damageMultiplier);
                }
                else if (enemyPrefab == EnemySpawnSettings.Instance.golemPrefab)
                {
                    enemyScript.healthMultiplier(EnemySpawnSettings.Instance.golemMultipliers.healthMultiplier);
                    enemyScript.speedMultiplier(EnemySpawnSettings.Instance.golemMultipliers.speedMultiplier);
                    enemyScript.damageMultiplier(EnemySpawnSettings.Instance.golemMultipliers.damageMultiplier);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
