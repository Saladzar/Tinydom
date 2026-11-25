using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] protected int health = 100, price = 100;
    [SerializeField] protected int[] upgradePrices;
    [SerializeField] protected GameObject debrisPrefab;

    protected BuildingType buildingType;
    protected int currentHealth { get; private set; }
    public int level { get; private set; }
    public bool isDestroyed { get; private set; }
    protected GameObject debris;

    protected void Init()
    {
        level = 1;
        currentHealth = health;
    }

    public int GetMaxHealth() { return health; }

    public int GetPrice() { return price; }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) HandleDestroyed();
    }

    protected void HandleDestroyed() 
    {
        isDestroyed = true;
        gameObject.SetActive(false);
        debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
    }

    public void Repair()
    {
        isDestroyed = false;
        gameObject.SetActive(true);
        Destroy(debris);
        currentHealth = health;
    }

    public int GetPriceForNextUpgrade() { return upgradePrices[level]; }

    public BuildingType GetBuildingType() { return buildingType; }

    public string GetBuildingName() { return null; }
}

public enum BuildingType
{
    Balista,
    Cannon,
    Catapult,
    Turret,
    Wall,
    HomeTower
}