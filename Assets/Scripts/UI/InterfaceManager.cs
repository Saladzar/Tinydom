using TMPro;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    [SerializeField] private GameObject interfacePanel;

    [Header("Home Tower")]
    [SerializeField] private GameObject homeTowerInterface;
    [SerializeField] private TextMeshProUGUI homeTowerLevel, homeTowerHealth, homeTowerUpgradePrice;

    [Header("Weapon Tower")]
    [SerializeField] private GameObject weaponTowerInterface;
    [SerializeField] private TextMeshProUGUI weaponTowerName, weaponTowerLevel, weaponTowerHealth, 
            weaponTowerDamage, weaponTowerRange, weaponTowerUpgradePrice, weaponTowerDestroyPrice;

    [Header("Crystal")]
    [SerializeField] private GameObject crystalInterface;
    [SerializeField] private TextMeshProUGUI crystalName, crystalTotal, crystalPerStage, crystalMinePrice, crystalDestroyPrice;

    private enum InterfaceType { HomeTower, WeaponTower, Crystal, None }
    private InterfaceType interfaceType;

    public bool interfaceOpen { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void OnUpgrade()
    {
        
    }

    public void OnRemove()
    {
        
    }

    public void OnMine()
    {
        GridTile _tempTile = (GridTile) TileSelection.Instance.currentTile;

        if (GameState.Instance.RemoveCrystals(_tempTile.crystal.minePrice))
        {
            SFXManager.Instance.PlayConfirmSound();
            CrystalManager.Instance.MineCrystal(_tempTile.crystal);
            ExitInterface();
        } else SFXManager.Instance.PlayErrorSound();
    }

    public void HandleHomeTower(Building homeTower)
    {
        Debug.Log("Interface Open!");
        interfaceType = InterfaceType.HomeTower;
        homeTowerLevel.text = homeTower.level + "";
        homeTowerHealth.text = homeTower.GetMaxHealth() + "";
        homeTowerUpgradePrice.text = homeTower.GetPriceForNextUpgrade() + " C";
        UpdateInterfaceDisplay();
    }

    public void HandleWeaponTower(WeaponTower weaponTower)
    {
        interfaceType = InterfaceType.WeaponTower;
        weaponTowerName.text = weaponTower.GetBuildingName();
        weaponTowerLevel.text = weaponTower.level + "";
        weaponTowerHealth.text = weaponTower.GetMaxHealth() + "";
        weaponTowerDamage.text = weaponTower.GetDamage() + "";
        weaponTowerRange.text = weaponTower.GetRange() + "";
        weaponTowerUpgradePrice.text = weaponTower.GetPriceForNextUpgrade() + " C";
        //weaponTowerDestroyPrice.text = "+" + weaponTower.GetPriceToDestroy() + " C";
        UpdateInterfaceDisplay();
    }

    public void HandleCrystal(Crystal crystal)
    {
        interfaceType = InterfaceType.Crystal;
        crystalName.text = crystal.isLarge ? "LARGE\nCRYSTAL" : "NORMAL\nCRYSTAL";
        crystalTotal.text = crystal.startingCrystalAmount + " C";
        crystalPerStage.text = crystal.crystalPerStage + " C";
        crystalMinePrice.text = crystal.minePrice + " C";
        crystalDestroyPrice.text = crystal.destroyPrice + " C";
        UpdateInterfaceDisplay();
    }

    public void ExitInterface()
    {
        interfaceType = InterfaceType.None;
        UpdateInterfaceDisplay();
        TileSelection.Instance.ExitMenu();
        SFXManager.Instance.PlayBackSound();
    }

    private void UpdateInterfaceDisplay()
    {
        homeTowerInterface.SetActive(interfaceType == InterfaceType.HomeTower);
        weaponTowerInterface.SetActive(interfaceType == InterfaceType.WeaponTower);
        crystalInterface.SetActive(interfaceType == InterfaceType.Crystal);
        interfacePanel.SetActive(interfaceType != InterfaceType.None);
        interfaceOpen = interfaceType != InterfaceType.None;
    }
}
