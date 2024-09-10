using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance;

    #region Variables
    [Header("Terrain Grid Settings")]
    [SerializeField][Range(10, 200)] private int gridSizeX = 50;
    [SerializeField][Range(10, 200)] private int gridSizeY = 50;
    [SerializeField][Range(1, 10)] private float tileSize = 2f;
    [SerializeField] private GameObject tilePrefab;

    [Space]
    [Header("Enemy Spawner Settings")]
    [SerializeField][Range(10, 100)] private float minSpawnDistance = 10f; // Minimum distance from center
    [SerializeField][Range(10, 100)] private float maxSpawnDistance = 20f; // Maximum distance from center
    [SerializeField] private GameObject enemySpawnPointPrefab;

    [Space]
    [Header("Main Tower Settings")]
    [SerializeField] private GameObject mainTowerPrefab;
    private GameObject mainTower;

    [Space]
    [Header("Path and Build Settings")]
    [SerializeField][Range(1, 10)] private int numberOfPaths = 3;
    [SerializeField][Range(1, 10)] private int pathWidth = 2;
    [SerializeField][Range(1, 10)] private int startRadius = 2;
    [SerializeField][Range(1, 10)] private int minDistanceBetweenPaths = 7;
    [SerializeField][Range(1, 10)] private int buildAreaWidth = 1;
    [SerializeField][Range(1, 50)] private float buildRadius = 20f;
    [SerializeField] private bool showBuildRadiusGizmo = true;

    [Space]
    [Header("Path Curvature Settings")]
    [SerializeField][Range(0.0f, 1.0f)] private float curvatureIntensity = 0.5f; // Controls how strong the curvature is
    [SerializeField][Range(0.0f, 1.0f)] private float curvatureFrequency = 0.1f; // Controls how often the path curves

    [Space]
    [Header("Random Radius Path Settings")]
    [SerializeField][Range(1, 10)] private int maxRadius = 6; // Maximum radius for the center
    [SerializeField][Range(1, 10)] private int minRadius = 2;  // Minimum radius for the center
    [SerializeField][Range(0.0f, 1.0f)] private float irregularity = 0f; // Controls shape irregularity (0 = perfect circle, 1 = highly irregular)
    [SerializeField][Range(0.0f, 1.0f)] private float density = 1f; // Controls how densely packed the center tiles are (0 = sparse, 1 = dense)

    [Space]
    [Header("Terrain Height Settings")]
    [SerializeField] private int flatRadius = 3; // Radius around build tiles where the terrain is flat
    [SerializeField] private float maxHeight = 5f; // Maximum height of the terrain
    [SerializeField] private float heightGradient = 1f; // Controls the steepness of the terrain
    [SerializeField] private AnimationCurve heightCurve; // Curve defining the height gradient

    [Space]
    [Header("Terrain Texture Settings")]
    [Tooltip("Threshold to determine steep slopes")]
    [SerializeField][Range(0f, 3f)] private float steepSlopeThreshold = 1.5f;
    [Tooltip("Minimum height for stones to appear")]
    [SerializeField][Range(0f, 5f)] private float minStoneHeightThreshold = 3.0f;
    [Tooltip("Maximum height for stones to appear")]
    [SerializeField][Range(0f, 10f)] private float maxStoneHeightThreshold = 4.8f;
    [Tooltip("Base probability of stone placement")]
    [SerializeField][Range(0.0f, 1.0f)] private float stonePlacementProbability = 0.3f;
    [Tooltip("Increased chance of stones near other stones")]
    [SerializeField][Range(0.0f, 1.0f)] private float stoneClusterProbability = 0.5f;
    [Tooltip("Random chance for rocks on non-steep areas")]
    [SerializeField][Range(0.0f, 1.0f)] private float rockRandomness = 0.2f;

    [Space]
    [Header("NavMeshSurface")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("Vegetation Settings")]
    [SerializeField] private List<GameObject> treePrefabs;
    [SerializeField] private List<GameObject> rockPrefabs;
    [SerializeField] private List<GameObject> bushPrefabs;


    //Gizmo
    private bool showSpawnRadiusGizmo = true;

    private List<Tile> grid = new List<Tile>();
    private List<Tile> enemySpawnLocations = new List<Tile>();
    private Vector2Int towerPosition;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        Singleton();
    }

    public void Start()
    {
        Generate();
    }

    #endregion

    #region Functions
    private void Generate()
    {
        ClearTerrain();
        GenerateTerrain();
        GeneratePaths();
        GenerateRandomRadiusPath();
        GenerateBuildAreas();
        ApplyTerrainHeight();
        ApplyTerrainTextures();
        SpawnStructures();
        TileVegetation();
        BakeNavMesh();
        if (Application.isPlaying)
            EnemySpawnSettings.Instance.UpdateEnemySpawners();
    }

    #region Terrain

    private void GenerateTerrain()
    {
        grid.Clear();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Set the tile position
                Vector3 tilePosition = new Vector3(x * tileSize, 0, y * tileSize);

                // Instantiate the tile prefab at the calculated position
                Tile newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity).GetComponent<Tile>();

                // Set the tile as a child of the TerrainGenerator
                newTile.transform.parent = this.transform;

                //Set the Tile x, y position
                newTile.SetPosition(x, y);

                // Set the TileType to Ground/Default
                newTile.SetTileType(Tile.TileType.Grass);

                //Rename Tile
                newTile.gameObject.name = $"Tile_{x},{y}_{newTile.GetTileType().ToString()}";

                // Add the tile to the grid list
                grid.Add(newTile);
            }
        }
    }

    public void ClearTerrain()
    {
        // Clear the list of enemy spawn locations 
        enemySpawnLocations.Clear();

        // Store the current children in a list to avoid modifying the collection while iterating
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        // Destroy each child object
        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        //Clear grid
        grid.Clear();

        //Clear Navmesh
        navMeshSurface.BuildNavMesh();
    }
    #endregion

    #region Paths
    private void GeneratePaths()
    {
        Vector2Int towerPosition = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        List<Vector2Int> usedStartingPoints = new List<Vector2Int>();

        for (int i = 0; i < numberOfPaths; i++)
        {
            Vector2Int startPosition = GetValidStartPosition(usedStartingPoints);
            usedStartingPoints.Add(startPosition);

            // Create a radius of path tiles around the start position
            CreatePathRadius(startPosition, startRadius);

            Vector2Int currentPos = startPosition;
            Vector2Int lastDirection = Vector2Int.zero;

            while (currentPos != towerPosition)
            {
                // Add randomness to the path's direction
                Vector2Int direction = GetCurvedDirection(currentPos, towerPosition, lastDirection);
                currentPos += direction;
                lastDirection = direction;

                CreatePathAtPosition(currentPos);

                // Ensure the path is continuous and consider the width
                for (int w = 0; w < pathWidth; w++)
                {
                    CreatePathAtPosition(new Vector2Int(currentPos.x + w, currentPos.y));
                    CreatePathAtPosition(new Vector2Int(currentPos.x, currentPos.y + w));
                }
            }

            // Add spawn location to List and change tile type
            SetSpawnLocation(startPosition);
        }
    }

    private Vector2Int GetValidStartPosition(List<Vector2Int> usedPoints)
    {
        List<Vector2Int> possibleStartPoints = new List<Vector2Int>();
        Vector2Int center = new Vector2Int(gridSizeX / 2, gridSizeY / 2);

        // Number of attempts to find a valid spawn point
        int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate a random angle and distance
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            // Calculate the x and y offsets
            int offsetX = Mathf.RoundToInt(Mathf.Cos(angle) * distance);
            int offsetY = Mathf.RoundToInt(Mathf.Sin(angle) * distance);

            // Calculate the random position
            Vector2Int randomPosition = new Vector2Int(center.x + offsetX, center.y + offsetY);

            // Ensure the position is within the grid bounds and not too close to used points
            if (IsValidPosition(randomPosition) && !IsTooCloseToUsedPoints(randomPosition, usedPoints))
            {
                possibleStartPoints.Add(randomPosition);
            }
        }

        // Ensure we have valid start points
        if (possibleStartPoints.Count > 0)
        {
            // Select a random start point from the valid points
            return possibleStartPoints[Random.Range(0, possibleStartPoints.Count)];
        }
        else
        {
            // If no valid points are found, log an error and return a fallback position
            Debug.LogError("No valid start points found within the specified spawn radius. Returning center.");
            return center;
        }
    }

    private bool IsTooCloseToUsedPoints(Vector2Int point, List<Vector2Int> usedPoints)
    {
        foreach (var usedPoint in usedPoints)
        {
            if (Vector2Int.Distance(point, usedPoint) < minDistanceBetweenPaths)
            {
                return true;
            }
        }
        return false;
    }

    private void CreatePathAtPosition(Vector2Int position)
    {
        if (IsValidPosition(position))
        {
            Tile currentTile = grid[position.x * gridSizeY + position.y];
            if (currentTile.GetTileType() != Tile.TileType.Path && currentTile.GetTileType() != Tile.TileType.EnemySpawn)
            {
                currentTile.SetTileType(Tile.TileType.Path);
            }
        }
    }

    void CreatePathRadius(Vector2Int center, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int position = new Vector2Int(center.x + x, center.y + y);
                if (Vector2Int.Distance(center, position) <= radius)
                {
                    CreatePathAtPosition(position);
                }
            }
        }
    }

    private void SetSpawnLocation(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeY)
        {
            Tile spawnTile = grid[position.x * gridSizeY + position.y];

            if (spawnTile != null)
            {
                spawnTile.SetTileType(Tile.TileType.EnemySpawn);
                enemySpawnLocations.Add(spawnTile);
            }
        }
    }

    private Vector2Int GetCurvedDirection(Vector2Int currentPos, Vector2Int targetPos, Vector2Int lastDirection)
    {
        Vector2Int direction = targetPos - currentPos;

        // Normalize direction
        if (direction.x != 0) direction.x /= Mathf.Abs(direction.x);
        if (direction.y != 0) direction.y /= Mathf.Abs(direction.y);

        // Introduce curvature
        if (Random.Range(0f, 1f) < curvatureFrequency)
        {
            // Apply curvature based on the intensity
            if (Random.Range(0f, 1f) < 0.5f)
                direction.x += Random.Range(-1, 2) * Mathf.RoundToInt(curvatureIntensity);
            else
                direction.y += Random.Range(-1, 2) * Mathf.RoundToInt(curvatureIntensity);

            // Ensure we don't deviate too far from the original direction
            direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        }

        // Avoid repeating the same direction to create more natural curves
        if (direction == lastDirection)
        {
            direction = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
        }

        return direction;
    }

    #endregion

    #region Middle Paths

    private void GenerateRandomRadiusPath()
    {
        // Calculate the center of the grid
        Vector2Int center = new Vector2Int(gridSizeX / 2, gridSizeY / 2);

        // Iterate over a square area around the center
        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector2Int position = new Vector2Int(center.x + x, center.y + y);

                // Check if this position is within grid bounds
                if (IsValidPosition(position))
                {
                    // Calculate distance from the center
                    float distanceFromCenter = Vector2Int.Distance(center, position);

                    // Determine if the tile should be part of the path based on the radius and irregularity
                    float probability = Mathf.InverseLerp(maxRadius, minRadius, distanceFromCenter) * (1 - irregularity * Random.Range(0f, 1f));

                    // Increase probability to favor Path tiles inside the radius
                    if (distanceFromCenter < maxRadius * 0.5f)
                    {
                        probability += 0.3f;
                    }
                    else if (distanceFromCenter < maxRadius * 0.75f)
                    {
                        probability += 0.15f;
                    }

                    if (distanceFromCenter <= maxRadius &&
                        distanceFromCenter >= minRadius &&
                        Random.Range(0f, 1f) < probability * density)
                    {
                        CreatePathAtPosition(position);
                    }
                }
            }
        }
    }

    #endregion

    #region BuildArea

    private void GenerateBuildAreas()
    {
        foreach (Tile tile in grid)
        {
            if (tile.GetTileType() == Tile.TileType.Path)
            {
                // Check adjacent tiles and mark them as Build if they are within the build radius
                for (int x = -buildAreaWidth; x <= buildAreaWidth; x++)
                {
                    for (int y = -buildAreaWidth; y <= buildAreaWidth; y++)
                    {
                        Vector2Int adjacentPos = new Vector2Int(tile.GetPosition().x + x, tile.GetPosition().y + y);
                        if (IsValidPosition(adjacentPos) && IsWithinBuildRadius(adjacentPos))
                        {
                            Tile adjacentTile = grid[adjacentPos.x * gridSizeY + adjacentPos.y];
                            if (adjacentTile.GetTileType() == Tile.TileType.Grass)
                            {
                                adjacentTile.SetTileType(Tile.TileType.Build);
                                BuildableTile componentRef = adjacentTile.AddComponent<BuildableTile>();
                                // NOTE: I'm adding this here since only buildable tiles should have this component, 
                                //   the current project structure would require a lot of unwarranted changes to accommodate a separate prefab for buildable tiles, so do this instead.
                            }
                        }
                    }
                }
            }
        }
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeY;
    }

    private bool IsWithinBuildRadius(Vector2Int position)
    {
        Vector2Int center = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        float distance = Vector2Int.Distance(center, position);
        return distance <= buildRadius; // Ensure buildRadius is a defined variable
    }

    #endregion

    #region Terrain Height

    private void ApplyTerrainHeight()
    {
        foreach (Tile tile in grid)
        {
            float height = 0f;
            Vector2Int position = tile.GetPosition();

            // Check for flat area around build tiles and enemy spawn points
            if (tile.GetTileType() == Tile.TileType.Build || tile.GetTileType() == Tile.TileType.EnemySpawn || IsWithinFlatRadius(position))
            {
                height = 0f; // Flat area
            }
            else
            {
                // Calculate distance from the nearest Build tile or EnemySpawn tile
                float distanceFromFlat = GetDistanceFromNearestFlatTile(position);

                // Use a smoother transition for height
                float normalizedDistance = distanceFromFlat / (flatRadius * 2f); // Normalize based on max distance
                height = heightCurve.Evaluate(normalizedDistance) * maxHeight;
            }

            // Apply height to the tile's position
            Vector3 newPosition = tile.transform.position;
            newPosition.y = Mathf.Lerp(0f, height, heightGradient); // Smooth transition
            tile.transform.position = newPosition;
        }
    }

    private bool IsWithinFlatRadius(Vector2Int position)
    {
        foreach (Tile tile in grid)
        {
            if (tile.GetTileType() == Tile.TileType.Build || tile.GetTileType() == Tile.TileType.EnemySpawn)
            {
                float distance = Vector2Int.Distance(tile.GetPosition(), position);
                if (distance <= flatRadius)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private float GetDistanceFromNearestFlatTile(Vector2Int position)
    {
        float minDistance = float.MaxValue;

        foreach (Tile tile in grid)
        {
            if (tile.GetTileType() == Tile.TileType.Build || tile.GetTileType() == Tile.TileType.EnemySpawn)
            {
                float distance = Vector2Int.Distance(tile.GetPosition(), position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        return minDistance;
    }


    #endregion

    #region Terrain Textures

    private void ApplyTerrainTextures()
    {
        foreach (Tile tile in grid)
        {
            // Preserve the existing tile types for Path, Building, and flat ground around paths
            if (tile.GetTileType() == Tile.TileType.Path || tile.GetTileType() == Tile.TileType.Build || IsWithinFlatRadius(tile.GetPosition()))
            {
                continue; // Skip texture assignment for these tiles
            }

            Vector2Int position = tile.GetPosition();
            float height = tile.transform.position.y;

            // Determine slope based on surrounding tiles
            float slope = CalculateSlope(tile, position);

            // Apply texture based on slope, height, and randomness
            if (slope > steepSlopeThreshold)
            {
                tile.SetTileType(Tile.TileType.Rock);
            }
            else if (height >= minStoneHeightThreshold && height <= maxStoneHeightThreshold && Random.value < stonePlacementProbability)
            {
                tile.SetTileType(Tile.TileType.Stones);

                // Increase chance of stones clustering together
                if (Random.value < stoneClusterProbability)
                {
                    PlaceNearbyStones(position);
                }
            }
            else if (Random.value < rockRandomness && tile.GetTileType() != Tile.TileType.Stones)
            {
                tile.SetTileType(Tile.TileType.Rock);
            }
            else
            {
                tile.SetTileType(Tile.TileType.Grass);
            }
        }
    }

    private float CalculateSlope(Tile tile, Vector2Int position)
    {
        float slope = 0f;

        Vector2Int[] neighbors = {
        new Vector2Int(position.x + 1, position.y),
        new Vector2Int(position.x - 1, position.y),
        new Vector2Int(position.x, position.y + 1),
        new Vector2Int(position.x, position.y - 1)
    };

        foreach (Vector2Int neighborPos in neighbors)
        {
            if (IsValidPosition(neighborPos))
            {
                Tile neighborTile = grid[neighborPos.x * gridSizeY + neighborPos.y];
                float heightDifference = Mathf.Abs(tile.transform.position.y - neighborTile.transform.position.y);
                slope = Mathf.Max(slope, heightDifference);
            }
        }

        return slope;
    }

    private void PlaceNearbyStones(Vector2Int position)
    {
        Vector2Int[] neighbors = {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        foreach (Vector2Int neighborPos in neighbors)
        {
            if (IsValidPosition(neighborPos) && Random.value < stoneClusterProbability)
            {
                Tile neighborTile = grid[neighborPos.x * gridSizeY + neighborPos.y];
                if (neighborTile.GetTileType() != Tile.TileType.Path && neighborTile.GetTileType() != Tile.TileType.Build)
                {
                    neighborTile.SetTileType(Tile.TileType.Stones);
                }
            }
        }
    }


    #endregion

    #region Spawn Structures

    private void SpawnStructures()
    {
        Vector2Int centerPosition = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        Vector3 centerWorldPosition = new Vector3(centerPosition.x * tileSize, 0, centerPosition.y * tileSize);

        // Spawn the Main Tower in the middle of the map
        GameObject tower = Instantiate(mainTowerPrefab, centerWorldPosition + new Vector3(0, 1.9f, 0), Quaternion.identity);
        tower.transform.parent = this.transform;
        mainTower = tower;

        // Calculate the average direction for the main tower's rotation
        Vector3 averageDirection = Vector3.zero;

        foreach (Tile spawnTile in enemySpawnLocations)
        {
            Vector3 spawnWorldPosition = spawnTile.transform.position;

            // Spawn the Enemy Spawner structure
            GameObject enemySpawner = Instantiate(enemySpawnPointPrefab, spawnWorldPosition + new Vector3(0, 1.9f, 0), Quaternion.identity);
            enemySpawner.transform.parent = this.transform;

            // Calculate direction towards the center
            Vector3 directionToCenter = (centerWorldPosition - spawnWorldPosition).normalized;

            // Rotate the Enemy Spawner towards the center
            enemySpawner.transform.rotation = Quaternion.LookRotation(directionToCenter);

            // Accumulate directions for average calculation
            averageDirection += directionToCenter;
        }

        // Set the main tower's rotation to face the average direction of the enemy spawners
        if (enemySpawnLocations.Count > 0)
        {
            averageDirection /= enemySpawnLocations.Count;

            // Apply the rotation directly with a 180-degree adjustment around the Y-axis
            mainTower.transform.rotation = Quaternion.LookRotation(averageDirection) * Quaternion.Euler(0, 180, 0);
        }
    }


    #endregion

    #region NavMesh

    private void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    #endregion

    #region Tower Look to Path

    public Tile GetClosestPathTile(Vector3 position)
    {
        Tile closestTile = null;
        float closestDistance = Mathf.Infinity;

        foreach (Tile tile in grid)
        {
            if (tile.GetTileType() == Tile.TileType.Path)
            {
                float distanceToTile = Vector3.Distance(position, tile.transform.position);
                if (distanceToTile < closestDistance)
                {
                    closestDistance = distanceToTile;
                    closestTile = tile;
                }
            }
        }

        return closestTile;
    }




    #endregion

    #region Vegetation

    private void TileVegetation()
    {
        foreach (Tile tile in grid)
        {
            if (tile.GetTileType() == Tile.TileType.Grass && Percentage(20f)) // 20% chance to add bushes
            {
                // Spawn Bushes on Grass Tiles occasionally
                SpawnVegetation(tile, bushPrefabs);
            }
            else if (tile.GetTileType() == Tile.TileType.Grass)
            {
                // Spawn Trees on Grass Tiles
                SpawnVegetation(tile, treePrefabs);
            }
            else if (tile.GetTileType() == Tile.TileType.Rock || tile.GetTileType() == Tile.TileType.Stones)
            {
                // Spawn Rocks on Rock or Stone Tiles
                SpawnVegetation(tile, rockPrefabs);
            }

        }
    }

    private void SpawnVegetation(Tile tile, List<GameObject> vegetationPrefabs)
    {
        if (vegetationPrefabs == null || vegetationPrefabs.Count == 0)
            return;

        // Randomly select a vegetation prefab
        GameObject selectedVegetation = vegetationPrefabs[Random.Range(0, vegetationPrefabs.Count)];

        // Get random spawn point
        Transform[] spawnPoints = tile.GetVegetationSpawnPoints();
        if (spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate vegetation at the spawn point
            GameObject vegetation = Instantiate(selectedVegetation, randomPoint.position, Quaternion.identity);
            vegetation.transform.parent = tile.transform; // Make it a child of the tile to move together

            // Apply random scale to the vegetation
            float randomScaleFactor = Random.Range(0.4f, 0.9f);
            vegetation.transform.localScale *= randomScaleFactor;
        }
    }

    public bool Percentage(float chance)
    {
        // Ensure that the percentage chance is clamped between 0 and 100
        chance = Mathf.Clamp(chance, 0f, 100f);

        // Generate a random float between 0 and 100
        float randomValue = Random.Range(0f, 100f);

        // Return true if the random value is less than or equal to the chance, otherwise return false
        return randomValue <= chance;
    }

    #endregion

    #endregion

    #region GetSet
    //Debug
    public void SetDebugMode(bool value)
    {
        showSpawnRadiusGizmo = value;
        OnDrawGizmos();
    }
    public bool GetDebugMode()
    {
        return showSpawnRadiusGizmo;
    }

    //Paths
    public int GetNumberOfSpawnPoints()
    {
        return numberOfPaths;
    }

    //MainTower
    public GameObject GetMainTower()
    {
        return mainTower;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (showSpawnRadiusGizmo)
        {
            // Calculate the center of the grid
            Vector3 center = new Vector3(gridSizeX * tileSize / 2, 0, gridSizeY * tileSize / 2);

            // Draw the minimum spawn distance circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, minSpawnDistance * tileSize);

            // Draw the maximum spawn distance circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, maxSpawnDistance * tileSize);
        }

        if (showBuildRadiusGizmo)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(new Vector3(towerPosition.x * tileSize, 0, towerPosition.y * tileSize), buildRadius);
        }
    }

    #endregion

    #region Singleton
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of TerrainGenerator was destroyed on creation!");
            return;
        }

        Instance = this;

    }
    #endregion
}
