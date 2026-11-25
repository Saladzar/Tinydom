using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Enemies")] 
    [SerializeField] private int zombieWeight;
    [SerializeField] private int skeletonWeight;
    [SerializeField] private int ghostWeight;
    [SerializeField] private int vampireWeight;
    [SerializeField] private int maxSpawnLocations;
    [SerializeField] private int spawnIncreaseFrequency;
    
    [Header("Prefabs")]
    [SerializeField] private Enemy zombiePrefab;
    [SerializeField] private Enemy skeletonPrefab;
    [SerializeField] private Enemy ghostPrefab;
    [SerializeField] private Enemy vampirePrefab;

    [Header("Difficulty Scaling")]
    [SerializeField] private float baseWeight;
    [SerializeField] private float growthFactor;

    private int stage => GameState.Instance.stage;
    public List<Vector3> spawnLocations { get; private set; }
    public Dictionary<EnemyType, int> enemiesToSpawn { get; private set; }
    private List<Enemy> enemies;

    private const int SKELETON_STAGE = 4;
    private const int GHOST_STAGE = 7;
    private const int VAMPIRE_STAGE = 10;
    private const float MIN_SPAWN_DISTANCE = 0.5f;
    private const float SPAWN_INTERVAL = 0.25f;
    private static readonly Vector2[] SPAWN_OFFSETS =
    {
        new Vector2(-0.3f, -0.3f),
        new Vector2(-0.3f, 0.3f),
        new Vector2(0.3f, -0.3f),
        new Vector2(0.3f, 0.3f)
    };

    private GridManager grid;
    private int pendingSpawns;

    void Awake()
    {
        Instance = this;
        spawnLocations = new List<Vector3>();
        enemiesToSpawn = new Dictionary<EnemyType, int>();
        enemies = new List<Enemy>();
    }

    public void PrepareNextSpawn()
    {
        if (grid == null)
            grid = GridManager.Instance;

        if (grid == null)
            throw new InvalidOperationException("EnemySpawner requires a GridManager.");

        spawnLocations.Clear();
        enemiesToSpawn.Clear();
        enemies.Clear();
        pendingSpawns = 0;

        int numSpawnLocations;

        switch (stage)
        {
            case 1:
                numSpawnLocations = 1;
                enemiesToSpawn[EnemyType.Zombie] = 1;
                break;

            case SKELETON_STAGE:
                numSpawnLocations = 1;
                enemiesToSpawn[EnemyType.Skeleton] = 3;
                break;

            case GHOST_STAGE:
                numSpawnLocations = 1;
                enemiesToSpawn[EnemyType.Ghost] = 5;
                break;

            case VAMPIRE_STAGE:
                numSpawnLocations = 1;
                enemiesToSpawn[EnemyType.Vampire] = 1;
                break;

            default:
                numSpawnLocations = (int)Math.Min(maxSpawnLocations, Math.Ceiling((double)stage / spawnIncreaseFrequency));

                int totalWeight = GetWaveWeight();

                int availableEnemies = 4;
                if (stage < SKELETON_STAGE) availableEnemies -= 3;
                else if (stage < GHOST_STAGE) availableEnemies -= 2;
                else if (stage < VAMPIRE_STAGE) availableEnemies -= 1;

                while (totalWeight > zombieWeight - 1)
                {
                    EnemyType randomEnemyType = (EnemyType) RandomNumberGenerator.GetInt32(0, availableEnemies);

                    switch (randomEnemyType)
                    {
                        case EnemyType.Zombie:
                            enemiesToSpawn.TryAdd(EnemyType.Zombie, 0);
                            enemiesToSpawn[EnemyType.Zombie]++;
                            totalWeight -= zombieWeight;
                            break;
                        
                        case EnemyType.Skeleton:
                            enemiesToSpawn.TryAdd(EnemyType.Skeleton, 0);
                            enemiesToSpawn[EnemyType.Skeleton]++;
                            totalWeight -= skeletonWeight;
                            break;
                        
                        case EnemyType.Ghost:
                            enemiesToSpawn.TryAdd(EnemyType.Ghost, 0);
                            enemiesToSpawn[EnemyType.Ghost]++;
                            totalWeight -= ghostWeight;
                            break;

                        case EnemyType.Vampire:
                            enemiesToSpawn.TryAdd(EnemyType.Vampire, 0);
                            enemiesToSpawn[EnemyType.Vampire]++;
                            totalWeight -= vampireWeight;
                            break;
                    }
                }

                break;
        }

        HashSet<Vector2Int> chosenSpawnTiles = new HashSet<Vector2Int>();
        for (int i = 0; i < numSpawnLocations; i++)
        {
            if (!grid.TryFindEmpty(chosenSpawnTiles, out Vector2Int gridPosition)) break;

            GridTile tile = grid.GetTile(gridPosition);
            spawnLocations.Add(tile.worldPosition + Vector3.up * 0.5f);
            chosenSpawnTiles.Add(gridPosition);
        }
    }

    public bool SpawnEnemies()
    {
        if (spawnLocations.Count == 0 || enemiesToSpawn.Count == 0)
            return false;

        List<Queue<EnemyType>> spawnQueues = new List<Queue<EnemyType>>(spawnLocations.Count);
        for (int i = 0; i < spawnLocations.Count; i++)
            spawnQueues.Add(new Queue<EnemyType>());

        foreach (var (type, num) in enemiesToSpawn)
        {
            for (int i = 0; i < num; i++)
            {
                int location = RandomNumberGenerator.GetInt32(0, spawnQueues.Count);
                spawnQueues[location].Enqueue(type);
                pendingSpawns++;
            }
        }

        if (pendingSpawns == 0)
            return false;

        StartCoroutine(SpawnEnemiesOverTime(spawnQueues));
        return true;
    }

    private IEnumerator SpawnEnemiesOverTime(List<Queue<EnemyType>> spawnQueues)
    {
        while (pendingSpawns > 0)
        {
            for (int i = 0; i < spawnQueues.Count; i++)
            {
                Queue<EnemyType> queue = spawnQueues[i];
                if (queue.Count == 0) continue;

                Vector3? spawnPosition = FindAvailableSpawnPosition(spawnLocations[i]);
                if (!spawnPosition.HasValue) continue;

                Enemy enemy = InstantiateEnemy(queue.Dequeue(), spawnPosition.Value);
                enemies.Add(enemy);
                pendingSpawns--;
            }

            if (pendingSpawns > 0)
                yield return new WaitForSeconds(SPAWN_INTERVAL);
        }
    }

    private Vector3? FindAvailableSpawnPosition(Vector3 location)
    {
        if (grid == null)
            grid = GridManager.Instance;

        float tileSize = grid.GetTileSize();
        Vector2Int? tilePosition = grid.WorldToGridPosition(location);
        if (!tilePosition.HasValue) return null;

        foreach (Vector2 offset in SPAWN_OFFSETS)
        {
            Vector3 position = location + new Vector3(offset.x * tileSize, 0f, offset.y * tileSize);
            Vector2Int? candidateTile = grid.WorldToGridPosition(position);
            if (!candidateTile.HasValue || candidateTile.Value != tilePosition.Value) continue;

            bool isOccupied = false;

            foreach (Enemy enemy in enemies)
            {
                if (enemy == null) continue;

                Vector3 separation = enemy.transform.position - position;
                separation.y = 0f;
                if (separation.sqrMagnitude < MIN_SPAWN_DISTANCE * MIN_SPAWN_DISTANCE)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied) return position;
        }

        return null;
    }

    private Enemy InstantiateEnemy(EnemyType type, Vector3 position)
    {
        switch (type)
        {
            case EnemyType.Zombie: return Instantiate(zombiePrefab, position, Quaternion.identity);
            case EnemyType.Skeleton: return Instantiate(skeletonPrefab, position, Quaternion.identity);
            case EnemyType.Ghost: return Instantiate(ghostPrefab, position, Quaternion.identity);
            case EnemyType.Vampire: return Instantiate(vampirePrefab, position, Quaternion.identity);
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private int GetWaveWeight()
    {
        float bossMuliplier = (stage % 5 == 0) ? 1.5f : 1.0f;
        float totalWeight = baseWeight * (1f + growthFactor * stage * stage) * bossMuliplier;

        return Mathf.RoundToInt(totalWeight);
    }

    public void UpdatePathfindingForAllEnemies()
    {
        foreach (Enemy enemy in enemies)
            enemy.Pathfind();
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) enemies.Remove(enemy);

        if (enemies.Count == 0 && pendingSpawns == 0) GameState.Instance.ProgressStage();
    }
}

public enum EnemyType
{
    Zombie = 0,
    Skeleton,
    Ghost,
    Vampire
}
