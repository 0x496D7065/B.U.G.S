using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffectManager : MonoBehaviour
{
    public static ImpactEffectManager Instance { get; private set; }

    [SerializeField] private List<ImpactEffectData> effectConfigs;

    private Dictionary<ImpactType, Queue<ParticleSystem>> poolMap = new();

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        foreach (var data in effectConfigs)
        {
            Queue<ParticleSystem> pool = new();
            for (int i = 0; i < data.poolSize; i++)
            {
                var effect = Instantiate(data.effectPrefab, transform);
                //effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                effect.gameObject.SetActive(false);
                pool.Enqueue(effect);
            }
            poolMap[data.type] = pool;
        }
    }

    public void PlayImpact(ImpactType type, RaycastHit hit, Vector3 normal)
    {
        if (!poolMap.TryGetValue(type, out var pool))
        {
            Debug.LogWarning($"[ImpactEffectManager] No pool for type: {type}");
            return;
        }

        ParticleSystem effect = pool.Dequeue();
        //effect.transform.SetPositionAndRotation(position, Quaternion.LookRotation(decalOffset));
        //effect.transform.SetParent(hit.collider.transform, false);
        Quaternion rotation = Quaternion.LookRotation(normal);
        effect.transform.SetPositionAndRotation(hit.point, rotation);
        effect.gameObject.SetActive(true);
        effect.Emit(1);
        
        StartCoroutine(ReturnAfter(effect, type, effect.main.duration + effect.main.startLifetime.constantMax));
    }

    private IEnumerator ReturnAfter(ParticleSystem ps, ImpactType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.Stop();
        ps.gameObject.SetActive(false);
        poolMap[type].Enqueue(ps);
    }
}
