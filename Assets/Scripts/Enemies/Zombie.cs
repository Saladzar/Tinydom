using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : Enemy
{
    void Start()
    {
        Init();

        Pathfind();
    }

    void FixedUpdate()
    {
        if (targetBuilding == null) return;

        if (targetBuilding.isDestroyed)
        {
            Pathfind();
            if (targetBuilding == null) return;
        }

        if (path == null || currentPathIndex >= path.Count) return;

        Vector2Int nextTile = path[currentPathIndex];
        GridTile tile = grid.GetTile(nextTile);

        if (tile.building != null && !tile.building.isDestroyed)
        {
            HandleBuilding(tile.building);
            return;
        }

        MoveTowards(tile.worldPosition);
    }

    public override void Pathfind()
    {
        TargetNearestBuilding();
        if (targetBuilding == null)
        {
            SetPath(null);
            return;
        }

        PathfindToTarget();
    }

    private void TargetNearestBuilding()
    {
        List<Building> buildings = BuildingPlacement.Instance.buildings;

        Building closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (Building building in buildings)
        {
            if (building == null || building.isDestroyed || building.GetBuildingType() == BuildingType.Wall) continue;

            float distance = Vector3.Distance(building.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBuilding = building;
            }
        }

        if (closestBuilding == null)
        {
            Building homeTower = GameState.Instance.homeTower;
            if (homeTower != null && !homeTower.isDestroyed)
                closestBuilding = homeTower;
        }

        targetBuilding = closestBuilding;
    }
}
