using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private ObjectPooler enemyPool;

    private EnemyAIManager aiManager;

    public float spawnRadius = 50f;
    public int spawnCount = 1;

    public void Start()
    {
        aiManager = GetComponent<EnemyAIManager>();
        for (int i = 0; i < spawnCount; i++)
            SpawnEnemy();
    }
    public void SpawnEnemy()
    {
        // Pick a random position around the spawner within a radius
        Vector3 spawnPos = transform.position + Random.onUnitSphere * spawnRadius;
        spawnPos.y = 1f; // Flatten to ground level

        GameObject enemyObj = enemyPool.GetFromPool(spawnPos, Quaternion.identity);
        EnemyAgent agent = enemyObj.GetComponent<EnemyAgent>();
        aiManager.RegisterEnemy(agent);
        agent.Init(aiManager);
    }
}