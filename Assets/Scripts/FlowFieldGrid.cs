using System;
using System.Collections.Generic;
using UnityEngine;

public static class FlowFieldUtils
{
    public static readonly Vector2Int[] Neighbors =
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1)
    };
}
public class FlowFieldGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector3 centerWorldPosition = Vector3.zero;
    private bool hasGenerated = false;

    private FlowFieldCell[,] grid;
    public void Generate(Vector3 targetPosition)
    {
        if (hasGenerated)
            return;
        hasGenerated = true;
        grid = new FlowFieldCell[width, height];
        Vector2Int targetIndex = WorldToCell(targetPosition);
        targetIndex.x = Mathf.Clamp(targetIndex.x, 0, width - 1);
        targetIndex.y = Mathf.Clamp(targetIndex.y, 0, height - 1);
        //Debug.Log($"Target Pos: {targetPosition} → Grid Index: {targetIndex}");

        // Breadth-first search (Dijkstra-style)
        //Debug.Log("here");
        grid[width - 1, height - 1] = new FlowFieldCell();
        //Debug.Log("here1");
        if (IsInBounds(targetIndex))
        {
            grid[targetIndex.x, targetIndex.y] = new FlowFieldCell { cost = 0 };
        }
        else
        {
            Debug.LogError($"❌ Target index out of bounds: {targetIndex}");
            return;
        }
        //Debug.Log("here2");

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(targetIndex);
        //Debug.Log("here3");
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            int currentCost = grid[current.x, current.y].cost;

            foreach (Vector2Int dir in FlowFieldUtils.Neighbors)
            {
                Vector2Int neighbor = current + dir;
                if (!IsInBounds(neighbor)) continue;
                if (IsObstacle(neighbor)) continue;

                var neighborCell = grid[neighbor.x, neighbor.y];
                if (neighborCell == null)
                {
                    neighborCell = new FlowFieldCell();
                    grid[neighbor.x, neighbor.y] = neighborCell;
                }

                if (neighborCell.cost <= currentCost + 1) continue;

                neighborCell.cost = currentCost + 1;
                frontier.Enqueue(neighbor);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int index = new Vector2Int(x, y);
                FlowFieldCell cell = grid[x, y];
                if (cell == null || IsObstacle(index))
                {
                    continue; 
                }

                int bestCost = cell.cost;
                Vector2 bestDir = Vector2.zero;

                List<Vector2> bestDirs = new List<Vector2>();
                foreach (Vector2Int dir in FlowFieldUtils.Neighbors)
                {
                    Vector2Int neighbor = index + dir;
                    if (!IsInBounds(neighbor)) continue;
                    if (IsObstacle(neighbor))continue;

                    FlowFieldCell neighborCell = grid[neighbor.x, neighbor.y];
                    if (neighborCell == null) continue;

                    if (neighborCell.cost < bestCost)
                    {
                        bestCost = neighborCell.cost;
                        bestDir = dir;
                    }
                }

                cell.direction = bestDir.normalized;
            }
        }
    }

    public Vector3 GetDirection(Vector3 worldPosition, Vector3 agentOffset)
    {
        Vector2Int index = WorldToCell(worldPosition);
        if (!IsInBounds(index)) return Vector3.zero;

        FlowFieldCell cell = grid[index.x, index.y];
        if (cell == null) return Vector3.zero;

        Vector3 baseDir = new Vector3(cell.direction.x, 0f, cell.direction.y).normalized;

        Vector3 bias = agentOffset * 0.1f; // weight of the offset
        return (baseDir + bias).normalized;
    }

    private Vector2Int WorldToCell(Vector3 pos)
    {
        Vector3 local = pos - GetGridOrigin();
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            Debug.LogWarning($"[FlowField] World pos {pos} → index ({x},{y}) out of bounds");
        }
        return new Vector2Int(x, y);
    }
    private Vector3 GetGridOrigin()
    {
        return centerWorldPosition - new Vector3(width * cellSize / 2f, 0f, height * cellSize / 2f);
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private bool IsObstacle(Vector2Int pos)
    {
        Vector3 center = GetGridOrigin() + new Vector3((pos.x + 0.5f) * cellSize, 1f, (pos.y + 0.5f) * cellSize);
        Vector3 halfExtents = new Vector3(cellSize * 0.5f, 1f, cellSize * 0.5f);

        bool hit = Physics.CheckBox(center, halfExtents, Quaternion.identity, obstacleLayer);
        if (hit)
        {
            Debug.DrawRay(center, Vector3.up * 2f, Color.red, 1f);
        }

        return hit;
        //return Physics.CheckBox(center, halfExtents, Quaternion.identity, obstacleLayer);
    }
    private Vector3 CellToWorld(Vector2Int cellIndex)
    {
        Vector3 origin = GetGridOrigin();
        return origin + new Vector3(
            (cellIndex.x + 0.5f) * cellSize,
            0f,
            (cellIndex.y + 0.5f) * cellSize
        );
    }
    private void OnDrawGizmosSelected()
    {
        if (grid == null) return;

        Gizmos.color = Color.black;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                FlowFieldCell cell = grid[x, y];
                if (cell == null) continue;

                Vector2Int cellIndex = new Vector2Int(x, y);
                Vector3 worldPos = CellToWorld(cellIndex);
                Vector3 direction = new Vector3(cell.direction.x, 0, cell.direction.y).normalized;
                if (IsObstacle(new Vector2Int(x, y)))
                {
                    Gizmos.DrawCube(worldPos, new Vector3(cellSize, 0.1f, cellSize));
                }

                Gizmos.DrawLine(worldPos, worldPos + direction * 0.5f);
                Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f); // optional: show cell centers
            }
        }
    }
}

public class FlowFieldCell
{
    public int cost = int.MaxValue;
    public Vector2 direction;
}