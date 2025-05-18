using UnityEngine;

public class AIDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    [Range(0, 180)] public float detectionAngle = 60f;
    public LayerMask targetLayerMask;

    [Header("Target Info (Read-only)")]
    public Transform currentTarget;

    void Update()
    {
        DetectTargets();
    }

    void DetectTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayerMask);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < detectionAngle)
            {
                currentTarget = hit.transform;
                Debug.DrawLine(transform.position, currentTarget.position, Color.red); // visual in scene view
                return;
            }
        }

        currentTarget = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
    }
}
