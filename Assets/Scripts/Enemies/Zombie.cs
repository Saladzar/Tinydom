using UnityEngine;

public class Zombie : Enemy
{
    void Start()
    {
        Init();
        targetBuilding = GameState.Instance.homeTower;

        Vector2Int gPos = (Vector2Int) grid.WorldToGridPosition(transform.position);
        Vector2Int targetGPos = (Vector2Int) grid.WorldToGridPosition(targetBuilding.transform.position);

        SetPath(Pathfinding.Instance.FindPath(gPos, targetGPos));
    }

    void FixedUpdate()
    {
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
}
