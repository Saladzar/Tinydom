using System;
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

    private GridManager grid;

    void Awake()
    {
        Instance = this;
        spawnLocations = new List<Vector3>();
        enemiesToSpawn = new Dictionary<EnemyType, int>();
        enemies = new List<Enemy>();
    }

    void Start()
    {
        grid = GridManager.Instance;
    }

    public void PrepareNextSpawn()
    {
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
                numSpawnLocations = (int)Math.Min(maxSpawnLocations, Math.Ceiling((double)(stage / spawnIncreaseFrequency)));

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

        for (int i = 0; i < numSpawnLocations; i++)
        {
            GridTile tile = grid.GetTile(grid.FindEmpty());
            spawnLocations.Add(tile.worldPosition + Vector3.up * 0.5f);
        }
    }

    public void SpawnEnemies()
    {
        foreach (var (type, num) in enemiesToSpawn)
        {
            for (int i = 0; i < num; i++)
            {
                int location = RandomNumberGenerator.GetInt32(0, spawnLocations.Count);

                switch (type)
                {
                    case EnemyType.Zombie: enemies.Add(Instantiate(zombiePrefab, spawnLocations[location], Quaternion.identity));
                        break;
                    case EnemyType.Skeleton: enemies.Add(Instantiate(skeletonPrefab, spawnLocations[location], Quaternion.identity));
                        break;
                    case EnemyType.Ghost: enemies.Add(Instantiate(ghostPrefab, spawnLocations[location], Quaternion.identity));
                        break;
                    case EnemyType.Vampire: enemies.Add(Instantiate(vampirePrefab, spawnLocations[location], Quaternion.identity));
                        break;
                }
            }
        }
    }

    private int GetWaveWeight()
    {
        float bossMuliplier = (stage % 5 == 0) ? 1.5f : 1.0f;
        float totalWeight = baseWeight * (1f + growthFactor * stage * stage) * bossMuliplier;

        return Mathf.RoundToInt(totalWeight);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) enemies.Remove(enemy);

        if (enemies.Count == 0) GameState.Instance.ProgressStage();
    }
}

public enum EnemyType
{
    Zombie = 0,
    Skeleton,
    Ghost,
    Vampire
}
