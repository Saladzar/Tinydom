using UnityEngine;

public class Vampire : Enemy
{
    void Start()
    {
        Init();
        
        Pathfind();
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

    public override void Pathfind()
    {
        targetBuilding = GameState.Instance.homeTower;

        PathfindToTarget();
    }
}
