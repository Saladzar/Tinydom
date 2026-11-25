using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    private readonly Dictionary<Building, Collider[]> buildingColliders = new();
    private readonly HashSet<Collider> ignoredBuildingColliders = new();
    private Collider[] ghostColliders;

    void Start()
    {
        Init();
        ghostColliders = GetComponentsInChildren<Collider>();
        Pathfind();
    }

    void FixedUpdate()
    {
        if (targetBuilding == null || targetBuilding.isDestroyed) return;

        IgnoreBuildingCollisions(targetBuilding);

        if (BuildingPlacement.Instance != null)
        {
            foreach (Building building in BuildingPlacement.Instance.buildings)
                IgnoreBuildingCollisions(building);
        }

        HandleBuilding(targetBuilding);
    }

    public override void Pathfind()
    {
        targetBuilding = GameState.Instance.homeTower;
    }

    private void IgnoreBuildingCollisions(Building building)
    {
        if (building == null) return;

        if (!buildingColliders.TryGetValue(building, out Collider[] obstacleColliders))
        {
            obstacleColliders = building.GetComponentsInChildren<Collider>(true);
            buildingColliders.Add(building, obstacleColliders);
        }

        foreach (Collider obstacleCollider in obstacleColliders)
        {
            if (obstacleCollider == null) continue;

            if (!obstacleCollider.enabled || !obstacleCollider.gameObject.activeInHierarchy)
            {
                ignoredBuildingColliders.Remove(obstacleCollider);
                continue;
            }

            if (!ignoredBuildingColliders.Add(obstacleCollider)) continue;

            foreach (Collider ghostCollider in ghostColliders)
                if (ghostCollider != null) Physics.IgnoreCollision(ghostCollider, obstacleCollider);
        }
    }
}
