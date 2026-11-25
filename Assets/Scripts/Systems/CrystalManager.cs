using System;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager Instance;

    [SerializeField] private GameObject normalCrystalPrefab, largeCrystalPrefab, crystalMinePrefab;
    [SerializeField] private int availableCrystalsBase = 300, availableCrystalsIncrease = 100,
            homeTowerCrystalsPerLevel = 20, homeTowerCrystalsPerStage = 5,
            normalCrystalEarnings = 10, largeCrystalEarnings = 20,
            normalCrystalLifetimeStages = 5, largeCrystalLifetimeStages = 8,
            normalCrystalMinePrice = 15, largeCrystalMinePrice = 35, crystalDestroyPrice = 10;

    private int stage => GameState.Instance.stage;
    private int availableCrystals => availableCrystalsBase + availableCrystalsIncrease * stage;
    private int normalCrystalAmount => normalCrystalEarnings * normalCrystalLifetimeStages;
    private int largeCrystalAmount => largeCrystalEarnings * largeCrystalLifetimeStages;
    private int currentCrystalsAvailable = 0;

    private List<Crystal> crystals;
    private GridManager gridManager;

    void Awake()
    {
        Instance = this;
        crystals = new List<Crystal>();
    }

    void Start()
    {
        gridManager = GridManager.Instance;
    }

    public void EarnCrystals()
    {
        int _homeTowerLevel = ((GridTile) GridManager.Instance.GetTileFromWorldPosition(Vector3.zero)).building.level;
        int _earnedCrystals = homeTowerCrystalsPerLevel * _homeTowerLevel + homeTowerCrystalsPerStage * GameState.Instance.stage;

        for (int i = crystals.Count - 1; i >= 0; i--)
        {
            Crystal crystal = crystals[i];
            if (crystal.isMined)
                _earnedCrystals += crystal.RemoveCrystals(crystal.crystalPerStage);
        }

        GameState.Instance.AddCrystals(_earnedCrystals);
        Debug.Log(_earnedCrystals);
    }

    public void SpawnCrystals()
    {
        if (gridManager == null)
            gridManager = GridManager.Instance;

        if (normalCrystalAmount <= 0 || largeCrystalAmount <= 0)
            return;

        int _crystalsToAdd = Mathf.Max(0, availableCrystals - currentCrystalsAvailable);
        int _numLarge = _crystalsToAdd / largeCrystalAmount;
        _crystalsToAdd -= _numLarge * largeCrystalAmount;
        int _numNormal = _crystalsToAdd / normalCrystalAmount;

        for (int i = 0; i < _numLarge; i++)
        {
            if (!TrySpawnCrystal(largeCrystalPrefab, true)) return;
        }

        for (int i = 0; i < _numNormal; i++)
        {
            if (!TrySpawnCrystal(normalCrystalPrefab, false)) return;
        }
    }

    private bool TrySpawnCrystal(GameObject prefab, bool isLarge)
    {
        List<GridTile> emptyTiles = new List<GridTile>();
        for (int x = 0; x < gridManager.GetGridSize(); x++)
        {
            for (int z = 0; z < gridManager.GetGridSize(); z++)
            {
                GridTile tile = gridManager.GetTile(x, z);
                if (tile.isWalkable) emptyTiles.Add(tile);
            }
        }

        if (emptyTiles.Count == 0) return false;

        GridTile spawnTile = emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)];
        Vector3 position = spawnTile.worldPosition + new Vector3(0f, 0.5f, 0f);
        GameObject crystalObject = Instantiate(prefab, position, Quaternion.identity);
        if (!crystalObject.TryGetComponent(out Crystal crystal)) Destroy(crystalObject);

        InitializeCrystal(crystal, isLarge);
        gridManager.PlaceOnTile(crystalObject);
        crystals.Add(crystal);
        currentCrystalsAvailable += isLarge ? largeCrystalAmount : normalCrystalAmount;
        return true;
    }

    private void InitializeCrystal(Crystal crystal, bool isLarge)
    {
        int amount = isLarge ? largeCrystalAmount : normalCrystalAmount;
        int earnings = isLarge ? largeCrystalEarnings : normalCrystalEarnings;
        int minePrice = isLarge ? largeCrystalMinePrice : normalCrystalMinePrice;

        crystal.InitializeCrystal(amount, isLarge, earnings, minePrice, crystalDestroyPrice);
    }

    public void MineCrystal(Crystal crystal)
    {
        if (crystal == null || crystal.isMined || !crystals.Contains(crystal))
            return;

        crystal.SetIsMined(true);
        Instantiate(crystalMinePrefab, crystal.transform.position, Quaternion.identity);
    }

    public void CrystalDestroyed(int amount)
    {
        currentCrystalsAvailable -= amount;
    }

    public void RemoveCrystal(Crystal crystal) { if (crystals.Contains(crystal)) crystals.Remove(crystal); }
}
