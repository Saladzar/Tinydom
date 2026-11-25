using System;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected int health = 100, price = 100;
    [SerializeField] protected int[] upgradePrices;
    [SerializeField] protected GameObject debrisPrefab;

    protected BuildingType buildingType;
    protected int currentHealth { get; private set; }
    public int level { get; private set; }
    public bool isDestroyed { get; private set; }
    protected GameObject debris;

    protected virtual void Init()
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

    protected virtual void HandleDestroyed() 
    {
        isDestroyed = true;
        gameObject.SetActive(false);
        GridManager.Instance.RemoveBuildingFromTile(this);
        debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
        Pathfinding.Instance.UpdatePathfinding();
        EnemySpawner.Instance.UpdatePathfindingForAllEnemies();
    }

    public virtual void Repair()
    {
        isDestroyed = false;
        gameObject.SetActive(true);
        Destroy(debris);
        currentHealth = health;
        GridManager.Instance.PlaceOnTile(gameObject);
        Pathfinding.Instance.UpdatePathfinding();
        EnemySpawner.Instance.UpdatePathfindingForAllEnemies();
    }

    public int GetPriceForNextUpgrade() { return upgradePrices[level]; }

    public BuildingType GetBuildingType() { return buildingType; }

    public abstract string GetBuildingName();

    public virtual void OnRemove()
    {
        GridManager.Instance.RemoveBuildingFromTile(this);
        BuildingPlacement.Instance.RemoveBuilding(this);
        Pathfinding.Instance.UpdatePathfinding();
        EnemySpawner.Instance.UpdatePathfindingForAllEnemies();
        Destroy(gameObject);
    }
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