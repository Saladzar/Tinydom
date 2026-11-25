using Unity.VisualScripting;
using UnityEngine;

public class Balista : WeaponTower
{
    void Start()
    {
        Init();
        
        buildingType = BuildingType.Balista;
    }

    public override string GetBuildingName() { return "BALISTA"; }
}
