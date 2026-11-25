using System;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager Instance;

    [SerializeField] private GameObject normalCrystalPrefab, largeCrystalPrefab, crystalMinePrefab;
    [SerializeField] private int availableCrystalsBase = 300, availableCrystalsIncrease = 100, homeTowerCrystalsPerLevel = 10,
            normalCrystalAmount = 150, largeCrystalAmount = 500, normalCrystalEarnings = 30, largeCrystalEarnings = 50;

    private int stage => GameState.Instance.stage;
    private int availableCrystals => availableCrystalsBase + availableCrystalsIncrease * stage;
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
        SpawnCrystals();
    }

    public void EarnCrystals()
    {
        int _homeTowerLevel = ((GridTile) GridManager.Instance.GetTileFromWorldPosition(Vector3.zero)).building.level;
        int _earnedCrystals = homeTowerCrystalsPerLevel * _homeTowerLevel * GameState.Instance.stage;

        foreach (Crystal crystal in crystals)
        {
            if (crystal.isMined)
                _earnedCrystals += crystal.isLarge ? largeCrystalEarnings : normalCrystalEarnings;
        }

        GameState.Instance.AddCrystals(_earnedCrystals);
        Debug.Log(_earnedCrystals);
    }

    public void SpawnCrystals()
    {
        int _crystalsToAdd = availableCrystals - currentCrystalsAvailable;

        int _numLarge = _crystalsToAdd / largeCrystalAmount;
        _crystalsToAdd -= _numLarge * largeCrystalAmount;

        int _numNormal = _crystalsToAdd / normalCrystalAmount;

        for (int i = 0; i < _numLarge; i++)
        {
            Vector3 pos = (Vector3) gridManager.GridToWorldPosition(gridManager.FindEmpty()) + new Vector3(0, 0.5f, 0);
            GameObject crystal = Instantiate(largeCrystalPrefab, pos, Quaternion.identity);
            gridManager.PlaceOnTile(crystal);
            crystals.Add(crystal.GetComponent<Crystal>());

            InitializeCrystal(crystal.GetComponent<Crystal>());
            currentCrystalsAvailable += largeCrystalAmount;
        }
        for (int i = 0; i < _numNormal; i++)
        {
            Vector3 pos = (Vector3) gridManager.GridToWorldPosition(gridManager.FindEmpty()) + new Vector3(0, 0.5f, 0);
            GameObject crystal = Instantiate(normalCrystalPrefab, pos, Quaternion.identity);
            gridManager.PlaceOnTile(crystal);
            crystals.Add(crystal.GetComponent<Crystal>());

            InitializeCrystal(crystal.GetComponent<Crystal>());
            currentCrystalsAvailable += normalCrystalAmount;
        }
    }

    private void InitializeCrystal(Crystal crystal)
    {
        throw new NotImplementedException();
    }

    public void MineCrystal(Crystal crystal)
    {
        if (crystal == null || !crystals.Contains(crystal))
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
