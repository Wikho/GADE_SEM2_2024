using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    #region Variables

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    [SerializeField] private int minGroupSize = 1;      //Min number of enemies in a group
    [SerializeField] private int maxGroupSize = 5;      //Max number of enemies in a group
    [SerializeField] private float minSpawnDelay = 1f;  //Min delay between groups
    [SerializeField] private float maxSpawnDelay = 3f;  //Max delay between groups
    private int totalWaveSize; 

    [Space]
    [Header("Enemy Attributes")]
    [SerializeField] private float enemyHealthMultiplier = 1.0f;
    [SerializeField] private float enemySpeedMultiplier = 1.0f;
    [SerializeField] private float enemyDamageMultiplier = 1.0f;

    private Transform enemySpawnParent;
    #endregion

    private void Awake()
    {
        //Find Place where all enemies will spawn
        enemySpawnParent = GameObject.FindGameObjectWithTag("Enemies").transform;
    }

    public void StartRound(int waveSize)
    {
        totalWaveSize = waveSize;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int enemiesRemaining = totalWaveSize;

        while (enemiesRemaining > 0)
        {
            int groupSize = Random.Range(minGroupSize, Mathf.Min(maxGroupSize, enemiesRemaining) + 1);
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);

            for (int i = 0; i < groupSize; i++)
            {
                SpawnEnemy();
                enemiesRemaining--;

                if (enemiesRemaining <= 0)
                    break;
            }

            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnEnemy()
    {
        //S{awn enemy
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        //Set Parent
        newEnemy.transform.parent = enemySpawnParent;

        //Apply Multipliers 
        newEnemy.GetComponent<EnemyController>().healthMultiplier(enemyHealthMultiplier);
        newEnemy.GetComponent<EnemyController>().speedMultiplier(enemySpeedMultiplier);
        newEnemy.GetComponent<EnemyController>().damageMultiplier(enemyDamageMultiplier);
    }
}
