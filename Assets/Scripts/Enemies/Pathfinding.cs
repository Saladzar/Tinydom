using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    [SerializeField] private int emptyTileCost = 1, buildingCost = 5, wallCost = 10;

    private int[,] tileCosts;
    private int gridSize;
    private GridManager gridManager;

    private Comparer<(int, Vector2Int)> comparer = Comparer<(int cost, Vector2Int coords)>.Create((a, b) =>
        {
            int result = a.cost.CompareTo(b.cost);
            if (result != 0) return result;
            result = a.coords.x.CompareTo(b.coords.x);
            if (result != 0) return result;
            return a.coords.y.CompareTo(b.coords.y);
        });
    
    private static Vector2Int[] DIRECTIONS =
    {
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1),
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 1,  1),
        new Vector2Int(-1,  1),
        new Vector2Int( 1, -1),
        new Vector2Int(-1, -1) 
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gridManager = GridManager.Instance;
        UpdatePathfinding();
    }

    public void UpdatePathfinding()
    {
        gridSize = gridManager.GetGridSize();
        tileCosts = new int[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
            {
                GridTile tile = gridManager.GetTile(x, y);

                if (tile.isWalkable)
                {
                    tileCosts[x, y] = emptyTileCost;
                    continue;
                }

                if (tile.building != null)
                {
                    if (tile.building.GetBuildingType() == BuildingType.Wall)
                    {
                        tileCosts[x, y] = wallCost;
                        continue;
                    }
                    else
                    {
                        tileCosts[x, y] = buildingCost;
                        continue;
                    }
                }

                if (tile.crystal != null) tileCosts[x, y] = int.MaxValue;
            }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        if (!IsInBounds(start) || !IsInBounds(end)) return null;

        int[,] minCosts = new int[gridSize, gridSize];
        Vector2Int?[,] parents = new Vector2Int?[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                minCosts[x, y] = int.MaxValue;

        if (tileCosts[start.x, start.y] == int.MaxValue)
            return null;

        var pq = new SortedSet<(int cost, Vector2Int coords)>(comparer);

        int startCost = tileCosts[start.x, start.y];
        minCosts[start.x, start.y] = startCost;
        pq.Add((startCost, start));

        while (pq.Count > 0)
        {
            var current = pq.Min;
            pq.Remove(current);

            Vector2Int coords = current.coords;
            if (current.cost != minCosts[coords.x, coords.y])
                continue;

            if (coords == end) return PathSmoother.Instance.SmoothPath(ReconstructPath(parents, end));

            for (int i = 0; i < DIRECTIONS.Length; i++)
            {
                Vector2Int newCoords = coords + DIRECTIONS[i];

                if (IsInBounds(newCoords))
                {
                    int tileCost = tileCosts[newCoords.x, newCoords.y];
                    if (tileCost == int.MaxValue)
                        continue;

                    long candidateCost = (long)current.cost + tileCost;
                    if (candidateCost >= int.MaxValue)
                        continue;

                    int newCost = (int)candidateCost;
                    if (newCost >= minCosts[newCoords.x, newCoords.y])
                        continue;

                    pq.Remove((minCosts[newCoords.x, newCoords.y], newCoords));
                    
                    minCosts[newCoords.x, newCoords.y] = newCost;
                    parents[newCoords.x, newCoords.y] = coords;
                    pq.Add((newCost, newCoords));
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Vector2Int?[,] parents, Vector2Int end)
    {
        var path = new List<Vector2Int>();
        Vector2Int? current = end;

        while (current != null)
        {
            path.Add(current.Value);
            current = parents[current.Value.x, current.Value.y];
        }

        path.Reverse();
        return path;
    }

    private bool IsInBounds(Vector2Int coords) 
            { return coords.x >= 0 && coords.y >= 0 && coords.x < gridSize && coords.y < gridSize; }
}
