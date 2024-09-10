using NUnit.Framework;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables
    [Header("Position")]
    [SerializeField] private Vector2Int position = new Vector2Int(0,0);

    [Space]
    [Header("Type")]
    [SerializeField] private TileType tileType = TileType.Grass;

    [Space]
    [Header("Vegetation Spawn Points")]
    [SerializeField] private Transform[] vegetationSpawns;

    public enum TileType
    {
        Grass,
        Path,
        Build,
        Rock,
        Stones,
        EnemySpawn,
        MainTower
    }

    [Space]
    [Header("Prefab")]
    [SerializeField] private GameObject[] groundPrefab;
    [SerializeField] private GameObject[] pathPrefab;
    [SerializeField] private GameObject[] buildPrefab;
    [SerializeField] private GameObject[] rockPrefab;
    [SerializeField] private GameObject[] stonesPrefab;
    [SerializeField] private GameObject[] enemySpawnerPrefab;
    [SerializeField] private GameObject[] mainTowerPrefab;

    #endregion

    #region Function

    private void UpdateTilePrefab() //This script gets updated in the Setter
    {
        // Check if there is an existing Tile/Child and then delete it
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "VegetationSpawns")
                {
                    //Dont Destroy
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        //Ground
        if (tileType == TileType.Grass)
        {
            if (groundPrefab != null)
            {
                Instantiate(groundPrefab[Random.Range(0,groundPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //Path
        if (tileType == TileType.Path)
        {
            if (pathPrefab != null)
            {
                Instantiate(pathPrefab[Random.Range(0, pathPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //Build
        if (tileType == TileType.Build)
        {
            if (buildPrefab != null)
            {
                Instantiate(buildPrefab[Random.Range(0, buildPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //Rock
        if (tileType == TileType.Rock)
        {
            if (rockPrefab != null)
            {
                Instantiate(rockPrefab[Random.Range(0, rockPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //Stones
        if (tileType == TileType.Stones)
        {
            if (stonesPrefab != null)
            {
                Instantiate(stonesPrefab[Random.Range(0, stonesPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //EnemySpawner
        if (tileType == TileType.EnemySpawn)
        {
            if (enemySpawnerPrefab != null)
            {
                Instantiate(enemySpawnerPrefab[Random.Range(0, enemySpawnerPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }

        //MainTower
        if (tileType == TileType.MainTower)
        {
            if (mainTowerPrefab != null)
            {
                Instantiate(mainTowerPrefab[Random.Range(0, mainTowerPrefab.Length)], transform.position, Quaternion.identity, transform);
            }
        }
    }

    #endregion

    #region GetSet

    public void SetPosition(int x, int y)
    {
        position = new Vector2Int(x, y);
    }

    public Vector2Int GetPosition()
    { 
        return position; 
    }

    public void SetTileType(TileType type)
    {
        tileType = type;
        gameObject.name = $"Tile_{position.x},{position.y}_{type.ToString()}";
        UpdateTilePrefab();
    }
    public TileType GetTileType()
    {
        return tileType;
    }

    public Transform[] GetVegetationSpawnPoints()
    {
        return vegetationSpawns;
    }

    #endregion
}
