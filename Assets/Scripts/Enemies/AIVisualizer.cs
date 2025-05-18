using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AIDetectionVisualizer : MonoBehaviour
{
    public int segments = 30;
    public float radius = 10f;
    public float angle = 60f;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.02f;
        lineRenderer.positionCount = segments + 2; // center + arc points
    }

    void Update()
    {
        DrawFOV();
    }

    void DrawFOV()
    {
        float startAngle = -angle;
        lineRenderer.SetPosition(0, Vector3.zero); // center point

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + (i * (angle * 2f / segments));
            float rad = Mathf.Deg2Rad * currentAngle;

            Vector3 point = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * radius;
            lineRenderer.SetPosition(i + 1, point);
        }
    }
}