using Unity.VisualScripting;
using UnityEngine;

public class HomeTower : Building
{
    void Start()
    {
        Init();
        buildingType = BuildingType.HomeTower;
    }

    protected override void HandleDestroyed()
    {
        gameObject.SetActive(false);
        debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
        GameState.Instance.OnGameOver();
    }

    public override void Repair() {}

    public override string GetBuildingName() { return "HOME TOWER"; }
}
