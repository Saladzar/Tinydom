using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    [SerializeField] private TextMeshProUGUI stageDisplay, crystalsDisplay;
    [SerializeField] private int startingCrystals = 100;
    [SerializeField] private GameObject homeTowerPrefab;

    public Building homeTower;
    public int stage { get; private set; }
    public int crystals { get; private set; }
    public bool inCombat { get; private set; }

    void Awake()
    {
        Init();
    }

    void Start()
    {
        homeTower = Instantiate(homeTowerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity).GetComponent<Building>();
        GridManager.Instance.PlaceOnTile(homeTower.gameObject);
        EnemySpawner.Instance.PrepareNextSpawn();
    }

    public void ProgressStage()
    {
        CrystalManager.Instance.EarnCrystals();
        EnemySpawner.Instance.PrepareNextSpawn();
        stage++;
        UpdateHUD();
    }

    public void AddCrystals(int amount)
    {
        crystals += amount;
        UpdateHUD();
    }

    public bool RemoveCrystals(int amount)
    {
        if (amount > crystals)
            return false;
        
        crystals -= amount;
        UpdateHUD();
        return true;
    }

    private void UpdateHUD()
    {
        stageDisplay.SetText(stage + "");
        crystalsDisplay.SetText(crystals + "");
    }

    private void Init()
    {
        Instance = this;
    
        stage = 1;
        crystals = startingCrystals;
        UpdateHUD();
    }
}
