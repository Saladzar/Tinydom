using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour
{
    public static BuildingPlacement Instance;
    private InputSystem_Actions inputActions;

    [SerializeField] private GameObject buildMenu, balistaPrefab, cannonPrefab, catapultPrefab, turretPrefab, wallPrefab;
    [SerializeField] private Image balistaPreview, cannonPreview, catapultPreview, turretPreview, wallPreview;
    [SerializeField] private TextMeshProUGUI balistaPriceText, cannonPriceText, catapultPriceText, turretPriceText, wallPriceText;
    [SerializeField] private float priceIncrease = 0.25f;

    private bool canPlace => buildingGhost != null && TileSelection.Instance.currentTile != null
            && ((GridTile)TileSelection.Instance.currentTile).isWalkable;
    private GameObject buildingGhost;
    private float buildingGhostAlpha = 0.5f;
    private int balistaPrice, cannonPrice, catapultPrice, turretPrice, wallPrice;
    public bool inMenu { get; private set; }
    public List<Building> buildings { get; private set; }

    void Awake()
    {
        Instance = this;
        buildings = new List<Building>();
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.Player.Select.performed += ctx => OnClick();
    }

    void Update()
    {
        if (!canPlace)
            return;

        buildingGhost.transform.position = ((GridTile)TileSelection.Instance.currentTile).worldPosition + new Vector3(0, 0.5f, 0);
    }

    public void BuildMenu()
    {
        inMenu = !inMenu;
        buildMenu.SetActive(inMenu);
        Destroy(buildingGhost);
        UpdateBuildMenu();
    }

    private void OnClick()
    {
        if (!canPlace)
        {
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        Vector3 pos = buildingGhost.transform.position;
        GameObject building = null;
        int price = 0;
        switch (buildingGhost.GetComponent<Building>().GetBuildingType())
        {
            case BuildingType.Balista:
                building = balistaPrefab;
                price = balistaPrice;
                break;
            case BuildingType.Cannon:
                building = cannonPrefab;
                price = cannonPrice;
                break;
            case BuildingType.Catapult:
                building = catapultPrefab;
                price = catapultPrice;
                break;
            case BuildingType.Turret:
                building = turretPrefab;
                price = turretPrice;
                break;
            case BuildingType.Wall:
                building = wallPrefab;
                price = wallPrice;
                break;
        }

        if (building == null || price < 0 || price > GameState.Instance.crystals)
        {
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        GameObject placedObject = Instantiate(building, pos, Quaternion.identity);
        if (!placedObject.TryGetComponent(out Building placedBuilding))
        {
            Destroy(placedObject);
            throw new InvalidOperationException($"Building prefab '{building.name}' has no Building component.");
        }

        if (!GameState.Instance.RemoveCrystals(price))
        {
            Destroy(placedObject);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        buildings.Add(placedBuilding);
        GridManager.Instance.PlaceOnTile(placedObject);
        SFXManager.Instance.PlayPlaceSound();

        UpdateBuildMenu();
    }

    private void UpdateBuildMenu()
    {
        balistaPrice = (int) (balistaPrefab.GetComponent<Building>().GetPrice() + 
                (priceIncrease * balistaPrefab.GetComponent<Building>().GetPrice() * CountBuildings(BuildingType.Balista)));
        cannonPrice = (int)(cannonPrefab.GetComponent<Building>().GetPrice() + 
                (priceIncrease * cannonPrefab.GetComponent<Building>().GetPrice() * CountBuildings(BuildingType.Cannon)));
        catapultPrice = (int)(catapultPrefab.GetComponent<Building>().GetPrice() + 
                (priceIncrease * catapultPrefab.GetComponent<Building>().GetPrice() * CountBuildings(BuildingType.Catapult)));
        turretPrice = (int)(turretPrefab.GetComponent<Building>().GetPrice() + 
                (priceIncrease * turretPrefab.GetComponent<Building>().GetPrice() * CountBuildings(BuildingType.Turret)));
        wallPrice = (int)(wallPrefab.GetComponent<Building>().GetPrice() + 
                (priceIncrease * wallPrefab.GetComponent<Building>().GetPrice() * CountBuildings(BuildingType.Wall)));

        balistaPriceText.text = balistaPrice + " C";
        cannonPriceText.text = cannonPrice + " C";
        catapultPriceText.text = catapultPrice + " C";
        turretPriceText.text = turretPrice + " C";
        wallPriceText.text = wallPrice + " C";

        if (balistaPrice > GameState.Instance.crystals) balistaPreview.color = Color.red;
        else balistaPreview.color = Color.white;

        if (cannonPrice > GameState.Instance.crystals) cannonPreview.color = Color.red;
        else cannonPreview.color = Color.white;

        if (catapultPrice > GameState.Instance.crystals) catapultPreview.color = Color.red;
        else catapultPreview.color = Color.white;

        if (turretPrice > GameState.Instance.crystals) turretPreview.color = Color.red;
        else turretPreview.color = Color.white;

        if (wallPrice > GameState.Instance.crystals) wallPreview.color = Color.red;
        else wallPreview.color = Color.white;
    }

    private int CountBuildings(BuildingType type)
    {
        int num = 0;

        foreach (Building building in buildings)
            if (building.GetBuildingType() == type) num++;

        return num;
    }

    public void OnBalista()
    {
        if (balistaPrice > GameState.Instance.crystals)
        {
            Destroy(buildingGhost);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(balistaPrefab);
    }

    public void OnCannon()
    {
        if (cannonPrice > GameState.Instance.crystals)
        {
            Destroy(buildingGhost);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(cannonPrefab);
    }

    public void OnCatapult()
    {
        if (catapultPrice > GameState.Instance.crystals)
        {
            Destroy(buildingGhost);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(catapultPrefab);
    }

    public void OnTurret()
    {
        if (turretPrice > GameState.Instance.crystals)
        {
            Destroy(buildingGhost);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(turretPrefab);
    }

    public void OnWall()
    {
        if (wallPrice > GameState.Instance.crystals)
        {
            Destroy(buildingGhost);
            SFXManager.Instance.PlayErrorSound();
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(wallPrefab);
    }

    public void RepairBuildings() { foreach (Building building in buildings) building.Repair(); }

    public void RemoveBuilding(Building building) { buildings.Remove(building); }

    private void InitializeBuildingGhost(GameObject building)
    {
        buildingGhost = Instantiate(building);
        Material mat = buildingGhost.GetComponentInChildren<MeshRenderer>().material;
        Color color = mat.color;
        color.a = buildingGhostAlpha;
        mat.color = color;
    }
}
