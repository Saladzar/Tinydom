using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSelection : MonoBehaviour
{
    public static TileSelection Instance;

    private InputSystem_Actions inputActions;

    [SerializeField] private GameObject selectionPrefab;
    [SerializeField] private Camera mainCamera;

    private Vector2 mousePosition => inputActions.Player.MousePosition.ReadValue<Vector2>();

    private GameObject selection;
    public GridTile currentTile { get; private set; }
    private bool isInMenu;

    void Awake()
    {
        Instance = this;

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.Player.Select.performed += ctx => OnClick();
    }

    void Start()
    {
        selection = Instantiate(selectionPrefab);
        selection.SetActive(false);
        isInMenu = false;
    }

    void Update()
    {
        if (isInMenu)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            currentTile = null;
            selection.SetActive(false);
            return;
        }

        Ray _ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(_ray, out RaycastHit hit, 500f))
        {
            currentTile = GridManager.Instance.GetTileFromWorldPosition(hit.point);

            if (currentTile == null)
                return;
            
            selection.SetActive(true);
            selection.transform.position = ((GridTile)currentTile).worldPosition + new Vector3(0, 0.5f, 0);
        }
        else
        {
            currentTile = null;
            selection.SetActive(false);
        }
    }

    private void OnClick()
    {
        if (currentTile == null || BuildingPlacement.Instance.inMenu)
            return;

        if (currentTile.isWalkable == true)
            return;

        if (currentTile.building != null)
        {
            SFXManager.Instance.PlayConfirmSound();
            if (currentTile.building.GetBuildingType() == BuildingType.HomeTower)
                InterfaceManager.Instance.HandleHomeTower(currentTile.building);
            else if (currentTile.building is WeaponTower)
                InterfaceManager.Instance.HandleWeaponTower((WeaponTower) currentTile.building);
            else if (currentTile.building is Wall){}
                // InterfaceManager.Instance.HandleWall((Wall) _tempTile.building);
        }

        if (currentTile.crystal != null)
            InterfaceManager.Instance.HandleCrystal(currentTile.crystal);
        
        isInMenu = true;
    }

    public void ExitMenu()
    {
        isInMenu = false;
    }
}
