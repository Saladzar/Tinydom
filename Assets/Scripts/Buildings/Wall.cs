using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : Building
{
    [SerializeField] private Mesh pillarMesh, endMesh, straightMesh, cornerMesh;
    [SerializeField] private GameObject firstMesh, secondMesh;

    private GridManager grid;

    private float[] offsets = { 0, 0.35f, 0.35f, 0.35f };

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    private List<Vector2Int> connections;

    void Start()
    {
        Debug.Log("placed!");
        Init();

        grid = GridManager.Instance;

        buildingType = BuildingType.Wall;
        connections = new List<Vector2Int>();
        ConfigureWall();
        ConfigureAdjacentWalls();
    }

    public void ConfigureWall()
    {
        connections.Clear();

        Vector2Int gridPosition = (Vector2Int) grid.WorldToGridPosition(transform.position);

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int adjacentTilePos = gridPosition + directions[i];
            if (!IsInsideGrid(adjacentTilePos)) continue;

            GridTile adjacentTile = grid.GetTile(adjacentTilePos);

            if (adjacentTile.building == null || adjacentTile.building.isDestroyed) continue;

            if (adjacentTile.building.GetBuildingType() == BuildingType.Wall)
                connections.Add(directions[i]);
        }

        float rotation = 0;

        switch (connections.Count)
        {
            case 0: 
                firstMesh.GetComponent<MeshFilter>().mesh = pillarMesh;
                secondMesh.SetActive(false);
                firstMesh.transform.localPosition = new Vector3(0, 0, offsets[0]);
                firstMesh.transform.localScale = new Vector3(2, 1, 2);
                break;

            case 1:
                firstMesh.GetComponent<MeshFilter>().mesh = endMesh;
                secondMesh.SetActive(false);
                firstMesh.transform.localPosition = new Vector3(0, 0, offsets[1]);
                firstMesh.transform.localScale = new Vector3(1, 1, 1);
                if (connections[0] == directions[0]) rotation = 90;
                else if (connections[0] == directions[1]) rotation = -90;
                else if (connections[0] == directions[3]) rotation = 180;
                break;

            case 2:
                secondMesh.SetActive(false);
                firstMesh.transform.localScale = new Vector3(1, 1, 1);
                if (connections[0] + connections[1] == Vector2Int.zero)
                {
                    firstMesh.GetComponent<MeshFilter>().mesh = straightMesh;
                    firstMesh.transform.localPosition = new Vector3(0, 0, offsets[2]);
                    if (connections.Contains(Vector2Int.up)) rotation = 90;
                    break;
                }
                else
                {
                    firstMesh.GetComponent<MeshFilter>().mesh = cornerMesh;
                    firstMesh.transform.localPosition = new Vector3(offsets[3], 0, offsets[3]);
                    Vector2Int diagonal = connections[0] + connections[1];
                    if (diagonal == new Vector2Int(-1, 1)) rotation = -90;
                    else if (diagonal == new Vector2Int(-1, -1)) rotation = 180;
                    else if (diagonal == new Vector2Int(1, -1)) rotation = 90;
                    break;
                }
            
            case 3:
                firstMesh.GetComponent<MeshFilter>().mesh = endMesh;
                firstMesh.transform.localPosition = new Vector3(0, 0, offsets[1]);
                firstMesh.transform.localScale = new Vector3(1, 1, 1);
                secondMesh.SetActive(true);
                List<Vector2Int> missingDirections = new List<Vector2Int>(directions);
                foreach (Vector2Int connection in connections)
                    missingDirections.Remove(connection);
                foreach (Vector2Int md in missingDirections)
                {
                    if (md == Vector2Int.up) rotation = -90;
                    else if (md == Vector2Int.left) rotation = 180;
                    else if (md == Vector2Int.down) rotation = 90;
                }
                break;

            case 4:
                firstMesh.GetComponent<MeshFilter>().mesh = straightMesh;
                firstMesh.transform.localPosition = new Vector3(0, 0, offsets[2]);
                firstMesh.transform.localScale = new Vector3(1, 1, 1);
                secondMesh.SetActive(true);
                break;
        }

        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    protected override void HandleDestroyed()
    {
        base.HandleDestroyed();
        ConfigureAdjacentWalls();
    }

    public override void Repair()
    {
        base.Repair();
        ConfigureWall();
        ConfigureAdjacentWalls();
    }

    public override void OnRemove()
    {
        grid.RemoveBuildingFromTile(this);
        ConfigureAdjacentWalls();
        base.OnRemove();
    }

    private void ConfigureAdjacentWalls()
    {
        Vector2Int gridPosition = (Vector2Int) grid.WorldToGridPosition(transform.position);

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int adjacentTilePos = gridPosition + directions[i];
            if (!IsInsideGrid(adjacentTilePos)) continue;

            GridTile adjacentTile = grid.GetTile(adjacentTilePos);

            if (adjacentTile.building == null || adjacentTile.building.isDestroyed) continue;

            if (adjacentTile.building.GetBuildingType() == BuildingType.Wall)
                adjacentTile.building.GetComponent<Wall>().ConfigureWall();
        }
    }

    private bool IsInsideGrid(Vector2Int position)
            { return position.x >= 0 && position.y >= 0 && position.x < grid.GetGridSize() && position.y < grid.GetGridSize(); }

    public override string GetBuildingName() { return "WALL"; }
}
