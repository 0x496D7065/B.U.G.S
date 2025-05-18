using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.Image;

[System.Serializable]
public class EnemyTrackingData
{
    public EnemyAgent agent;
    public Transform lastSeenPlayer;
    public float memoryDuration;
    public float lastSeenTime;
    public float nextDetectionTime;

    public EnemyTrackingData(EnemyAgent agent)
    {
        this.agent = agent;
        this.lastSeenPlayer = null;
        this.memoryDuration = 5f;
        this.lastSeenTime = 0f;
    }
}
public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance;

    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public GridController flowField;
    [SerializeField] private Transform baseTarget;

    public List<Transform> allPlayers = new();
    private List<EnemyTrackingData> trackedEnemies = new List<EnemyTrackingData>();

    private void Awake()
    {
        Instance = this;
        flowField.InitializeFlowField(baseTarget.position);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            allPlayers.Add(go.transform);
    }
    private void Update()
    {
        foreach (var data in trackedEnemies)
        {
            if (data.agent == null) continue;
            if (Time.time >= data.nextDetectionTime)
            {
                Transform nearestPlayer = GetNearestPlayerInRange(data.agent, playerDetectionRadius);
                data.nextDetectionTime = Time.time + 0.5f;
                if (nearestPlayer != null)
                {
                    // See a player? Chase and reset memory
                    data.lastSeenPlayer = nearestPlayer;
                    data.lastSeenTime = Time.time;
                    data.agent.SetTarget(nearestPlayer);
                }
                else if (Time.time - data.lastSeenTime < data.memoryDuration)
                {
                    // Player out of range, but memory still valid
                    data.agent.SetTarget(data.lastSeenPlayer);
                }
                else
                {
                    // No player seen, memory expired
                    data.agent.BackToFlowField();
                }
            }
        }
    }
    public Transform GetNearestPlayerInRange(EnemyAgent agent, float radius)
    {
        Transform closest = null;
        float minDist = radius;
        Vector3 origin = agent.transform.position + Vector3.up * 1f;
        foreach (Transform player in allPlayers)
        {
            float dist = Vector3.Distance(origin, player.position);
            if (dist < minDist)
            {
                 closest = player;
                 minDist = dist;
            }
        }

        return closest;
    }

    public void RegisterEnemy(EnemyAgent agent)
    {
        trackedEnemies.Add(new EnemyTrackingData(agent)
        {
            lastSeenTime = -999f,
            memoryDuration = 5f
        });
    }

    public void UnregisterEnemy(EnemyAgent agent)
    {
        trackedEnemies.RemoveAll(e => e.agent == agent);
    }
}