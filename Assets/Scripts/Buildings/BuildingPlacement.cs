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

    [SerializeField] private GameObject buildMenu, balistaPrefab, cannonPrefab, catapultPrefab, turretPrefab;
    [SerializeField] private Image balistaPreview, cannonPreview, catapultPreview, turretPreview;
    [SerializeField] private TextMeshProUGUI balistaPriceText, cannonPriceText, catapultPriceText, turretPriceText;
    [SerializeField] private float priceIncreaseRate = 1.4f;

    private bool canPlace => buildingGhost != null && TileSelection.Instance.currentTile != null
            && ((GridTile)TileSelection.Instance.currentTile).isWalkable;
    private GameObject buildingGhost;
    private float buildingGhostAlpha = 0.5f;
    private int balistaPrice, cannonPrice, catapultPrice, turretPrice;
    public bool inMenu { get; private set; }
    private List<Building> buildings;

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

        SFXManager.Instance.PlayPlaceSound();
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
        }

        GridManager.Instance.PlaceOnTile(Instantiate(building, pos, Quaternion.identity));
        GameState.Instance.RemoveCrystals(price);

        UpdateBuildMenu();
    }

    private void UpdateBuildMenu()
    {
        balistaPrice = (int)(balistaPrefab.GetComponent<Building>().GetPrice() *
                Math.Pow(priceIncreaseRate, CountBuildings(BuildingType.Balista)));
        cannonPrice = (int)(cannonPrefab.GetComponent<Building>().GetPrice() *
                Math.Pow(priceIncreaseRate, CountBuildings(BuildingType.Cannon)));
        catapultPrice = (int)(cannonPrefab.GetComponent<Building>().GetPrice() *
                Math.Pow(priceIncreaseRate, CountBuildings(BuildingType.Catapult)));
        turretPrice = (int)(turretPrefab.GetComponent<Building>().GetPrice() *
                Math.Pow(priceIncreaseRate, CountBuildings(BuildingType.Turret)));

        balistaPriceText.text = balistaPrice + " C";
        cannonPriceText.text = cannonPrice + " C";
        catapultPriceText.text = catapultPrice + " C";
        turretPriceText.text = turretPrice + " C";

        if (balistaPrice > GameState.Instance.crystals) balistaPreview.color = Color.red;
        else balistaPreview.color = Color.white;

        if (cannonPrice > GameState.Instance.crystals) cannonPreview.color = Color.red;
        else cannonPreview.color = Color.white;

        if (catapultPrice > GameState.Instance.crystals) catapultPreview.color = Color.red;
        else catapultPreview.color = Color.white;

        if (turretPrice > GameState.Instance.crystals) turretPreview.color = Color.red;
        else turretPreview.color = Color.white;
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
            return;
        }

        if (buildingGhost != null)
            Destroy(buildingGhost);

        InitializeBuildingGhost(turretPrefab);
    }

    public void OnWall()
    {

    }

    private void InitializeBuildingGhost(GameObject building)
    {
        buildingGhost = Instantiate(building);
        Material mat = buildingGhost.GetComponentInChildren<MeshRenderer>().material;
        Color color = mat.color;
        color.a = buildingGhostAlpha;
        mat.color = color;
    }
}
