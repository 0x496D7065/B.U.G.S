using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField curFlowField;
    private bool hasGenerated = false;

    public void InitializeFlowField(Vector3 baseTarget)
    {
        if (hasGenerated)
            return;
        hasGenerated = true;
        curFlowField = gameObject.AddComponent<FlowField>();
        curFlowField.Init(cellRadius, gridSize);
        curFlowField.CreateGrid();
        curFlowField.CreateCostField();
        Cell destinationCell = curFlowField.GetCellFromWorldPos(baseTarget);
        curFlowField.CreateIntegrationField(destinationCell);
        curFlowField.CreateFlowField();
    }

    private void OnDrawGizmosSelected()
    {
        if (curFlowField == null || curFlowField.grid == null) return;

        Gizmos.color = Color.black;

        for (int x = 0; x < curFlowField.grid.GetLength(0); x++)
        {
            for (int y = 0; y < curFlowField.grid.GetLength(1); y++)
            {
                Cell cell = curFlowField.grid[x, y];
                if (cell == null) continue;

                Vector3 worldPos = cell.worldPos;
                Vector3 direction = new Vector3(cell.bestDirection.Vector.x, 0, cell.bestDirection.Vector.y).normalized;

                Gizmos.DrawLine(worldPos, worldPos + direction * 0.5f);
                Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f); // optional: show cell centers
            }
        }
    }
}
