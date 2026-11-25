using UnityEngine;

public class Cannon : WeaponTower
{
    void Start()
    {
        Init();
        
        buildingType = BuildingType.Cannon;
    }

    public override string GetBuildingName() { return "CANNON"; }
}
