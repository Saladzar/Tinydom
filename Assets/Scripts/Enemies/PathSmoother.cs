
using System.Collections.Generic;
using UnityEngine;

public class PathSmoother : MonoBehaviour
{
    public static PathSmoother Instance;
    private GridManager gridManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gridManager = GridManager.Instance;
    }

    public List<Vector2Int> SmoothPath(List<Vector2Int> path)
    {
        if (path == null || path.Count <= 2) return path;

        List<Vector2Int> smoothedPath = new List<Vector2Int>();

        int currentIndex = 0;

        smoothedPath.Add(path[currentIndex]);

        while (currentIndex < path.Count - 1)
        {
            int furthestVisible = currentIndex + 1;

            for (int i = path.Count - 1; i > currentIndex + 1; i--)
            {
                if (HasClearPath(path[currentIndex], path[i]))
                {
                    furthestVisible = i;
                    break;
                }
            }

            currentIndex = furthestVisible;
            smoothedPath.Add(path[currentIndex]);
        }

        return smoothedPath;
    }

    private bool HasClearPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> tiles = GetLineTiles(start, end);

        foreach (Vector2Int tilePosition in tiles)
            if (gridManager.GetTile(tilePosition.x, tilePosition.y) == null ||
                    !gridManager.GetTile(tilePosition.x, tilePosition.y).isWalkable)
                return false;
        
        return true;
    }

    private List<Vector2Int> GetLineTiles(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        int x = start.x;
        int y = start.y;

        int dx = Mathf.Abs(end.x - x);
        int dy = Mathf.Abs(end.y - y);

        int sx = x < end.x ? 1 : -1;
        int sy = y < end.y ? 1 : -1;

        int error = dx - dy;

        while (true)
        {
            cells.Add(new Vector2Int(x, y));

            if (x == end.x && y == end.y) break;

            int error2 = 2 * error;

            if (error2 > -dy)
            {
                error -= dy;
                x += sx;
            }

            if (error2 < dx)
            {
                error += dx;
                y += sy;
            }
        }

        return cells;
    }
}
