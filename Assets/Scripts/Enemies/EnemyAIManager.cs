using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using Unity.VisualScripting;

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

    private void Awake()
    {
        Instance = this;
        flowField.InitializeFlowField(baseTarget.position);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            allPlayers.Add(go.transform);
        obstructionMask = LayerMask.GetMask("Obstacles", "Player");
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

    public Transform GetNearestPlayerInRange(EnemyAgent agent, float radius)
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