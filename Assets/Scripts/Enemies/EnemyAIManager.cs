using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using Unity.VisualScripting;
using static EnemyAgent;
using System.Drawing;

[System.Serializable]
public class EnemyTrackingData
{
    public EnemyAgent agent;
    //public Transform lastSeenPlayer;
    //public float memoryDuration;
    //public float lastSeenTime;
    //public float nextDetectionTime;

    public EnemyTrackingData(EnemyAgent agent)
    {
        this.agent = agent;
        //this.lastSeenPlayer = null;
        //this.memoryDuration = 5f;
        //this.lastSeenTime = 0f;
    }
}
public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public GridController flowField;
    [SerializeField] private Transform baseTarget;
    private int agentsPerFrame;
    private int detectionIndex = 0;

    public List<Transform> allPlayers = new();
    private List<EnemyTrackingData> trackedEnemies = new List<EnemyTrackingData>();

    public LayerMask obstructionMask;
    public LayerMask targetMask;

    private void Awake()
    {
        Instance = this;
        flowField.InitializeFlowField(baseTarget.position);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            allPlayers.Add(go.transform);
        //obstructionMask = LayerMask.GetMask("Obstacles", "Player");
        obstructionMask = LayerMask.GetMask("Obstacles", "Player", "Base", "Construction");
        targetMask = LayerMask.GetMask("Player", "Base", "Construction");
    }
    private void Update()
    {
        agentsPerFrame = Mathf.Clamp(trackedEnemies.Count / 5, 10, 50);
        for (int i = 0; i < agentsPerFrame; i++)
        {
            if (trackedEnemies.Count == 0) return;
            detectionIndex %= trackedEnemies.Count;
            var agent = trackedEnemies[detectionIndex].agent;
            if (agent != null)
            {
                agent.TickDetection();
            }
            detectionIndex++;
        }
    }

    /*public Transform GetNearestVisiblePlayerInRange(EnemyAgent agent, float radius)
    {
        Transform closest = null;
        float minDist = radius;
        Vector3 origin = agent.transform.position + Vector3.up * 1f;
        foreach (Transform player in allPlayers)
        {
            Vector3 direction = (player.position - origin).normalized;
            float dist = Vector3.Distance(origin, player.position);
            //Debug.Log($"dist= {dist}");
            //Debug.Log($"minDist= {minDist}");
            if (dist < minDist)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, dist, obstructionMask))
                {
                    Debug.Log($"Raycast hit: {hit.transform.name}");
                    if (hit.transform == player || hit.transform.IsChildOf(player))
                    {
                        closest = player;
                        minDist = dist;
                    }
                }
                else
                    Debug.Log("Raycast did not hit anything.");
            }
        }
        return closest;
    }*/
    public TargetInfo? GetNearestVisibleTargetInRange(EnemyAgent agent, float radius)
    {
        Transform closestTransform = null;
        Vector3 closestPoint = Vector3.zero;
        float minDist = radius;
        Vector3 origin = agent.transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapSphere(origin, radius, targetMask);
        foreach (Collider hit in hits)
        {
            Transform target = hit.transform;
            Vector3 targetPoint = hit.ClosestPoint(origin);
            Vector3 direction = (targetPoint - origin).normalized;
            float dist = Vector3.Distance(origin, targetPoint);
            if (Physics.Raycast(origin, direction, out RaycastHit raycastHit, dist, obstructionMask))
            {
                //Debug.Log($"raycasthit = {raycastHit.transform.name}");
                if (raycastHit.transform == target || raycastHit.transform.IsChildOf(target))
                {
                    if (dist < minDist)
                    {
                        closestTransform = target;
                        closestPoint = targetPoint;
                        minDist = dist;
                    }
                }
            }
        }
        if (closestTransform != null)
            return new TargetInfo { targetTransform = closestTransform, closestPoint = closestPoint };
        return null;
    }
    public void RegisterEnemy(EnemyAgent agent)
    {
        trackedEnemies.Add(new EnemyTrackingData(agent){});
        agent.OnDeath += HandleEnemyDeath;
    }
    private void HandleEnemyDeath(EnemyAgent agent)
    {
        UnregisterEnemy(agent);
    }
    public void UnregisterEnemy(EnemyAgent agent)
    {
        agent.OnDeath -= HandleEnemyDeath;
        trackedEnemies.RemoveAll(e => e.agent == agent);
    }
}