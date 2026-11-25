using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int gridSize = 15;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private Transform terrain;

    private GridTile[,] grid;

    private int gridApothem => (gridSize - 1) / 2;

    void Awake()
    {
        Instance = this;
        grid = new GridTile[gridSize, gridSize];
    }

    void OnEnable()
    {
        for (int x = -gridApothem; x <= gridApothem; x++)
        {
            for (int z = -gridApothem; z <= gridApothem; z++)
            {
                Vector3 wPos = new Vector3(x * tileSize, 0, z * tileSize);
                Vector2Int gPos = new Vector2Int(x + gridApothem, z + gridApothem);
                grid[gPos.x, gPos.y] = new GridTile(wPos, gPos);
                Instantiate(terrainPrefab, wPos, Quaternion.identity, terrain);
            }
        }
    }

    void Start()
    {
        TerrainGenerator.Instance.Generate();
    }

    public int GetGridSize() { return gridSize; }

    public float GetTileSize() { return tileSize; }

    public GridTile GetTile(int x, int z) { return grid[x, z]; }

    public GridTile GetTile(Vector2Int coords) { return grid[coords.x, coords.y]; }

    public GridTile GetTileFromWorldPosition(Vector3 pos)
    {
        if (IsOutOfBounds(pos))
            return null;

        int x = (int) MathF.Round((pos.x + gridApothem) / tileSize);
        int z = (int) MathF.Round((pos.z + gridApothem) / tileSize);

        return grid[x,z];
    }

    public Vector2Int? WorldToGridPosition(Vector3 pos)
    {
        if (IsOutOfBounds(pos)) return null;

        int x = (int) MathF.Round((pos.x + gridApothem) / tileSize);
        int z = (int) MathF.Round((pos.z + gridApothem) / tileSize);

        return new Vector2Int(x, z);
    }

    public Vector3? GridToWorldPosition(Vector2Int pos)
    {
        if (IsOutOfBounds(pos))
            return null;

        return grid[pos.x, pos.y].worldPosition;
    }

    public Vector2Int FindEmpty()
    {
        List<Vector2Int> _listOfEmpty = new List<Vector2Int>();

        foreach (GridTile tile in grid)
        {
            if (tile.isWalkable)
                _listOfEmpty.Add(tile.gridPosition);
        }

        return _listOfEmpty[UnityEngine.Random.Range(0, _listOfEmpty.Count)];
    }

    public void PlaceOnTile(GameObject gameObject)
    {
        if (WorldToGridPosition(gameObject.transform.position) == null)
            return;

        Vector2Int gPos = (Vector2Int) WorldToGridPosition(gameObject.transform.position);

        if (IsOutOfBounds(gPos))
            return;

        if (gameObject.TryGetComponent(out Building building))
            grid[gPos.x, gPos.y].building = building;
        else if (gameObject.TryGetComponent(out Crystal crystal))
            grid[gPos.x, gPos.y].crystal = crystal;
    }

    private bool IsOutOfBounds(Vector2Int gPos)
            { return gPos.x < 0 || gPos.y < 0 || gPos.x >= gridSize || gPos.y >= gridSize; }

    private bool IsOutOfBounds(Vector3 wPos)
            { return wPos.x < -gridApothem || wPos.x > gridApothem || wPos.z < -gridApothem || wPos.z > gridApothem; }
}
