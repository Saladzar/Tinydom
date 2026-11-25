using UnityEngine;

public class Catapult : WeaponTower
{
    void Start()
    {
        Init();
        
        buildingType = BuildingType.Catapult;
    }

    public override string GetBuildingName() { return "CATAPULT"; }
}
