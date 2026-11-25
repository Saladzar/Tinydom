using UnityEngine;

public class GridTile
{
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    public Building building;
    public Crystal crystal;
    public bool isWalkable => building == null && crystal == null;

    public GridTile(Vector3 position, Vector2Int gridPosition)
    {
        this.worldPosition = position;
        this.gridPosition = gridPosition;
        building = null;
        crystal = null;
    }
}
