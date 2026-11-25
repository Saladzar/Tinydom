using UnityEngine;

public class Turret : WeaponTower
{
    void Start()
    {
        Init();
        
        buildingType = BuildingType.Turret;
    }

    public override string GetBuildingName() { return "TURRET"; }
}
