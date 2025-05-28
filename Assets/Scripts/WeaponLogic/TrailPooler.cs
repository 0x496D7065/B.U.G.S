using System.Collections.Generic;
using UnityEngine;

public class TrailPooler : MonoBehaviour
{
    public TrailRenderer prefab;
    public int poolSize = 20;

    private Queue<TrailRenderer> pool = new Queue<TrailRenderer>();

    void Start()
    {
        Debug.Log($"[Pooler] Initializing pool on {gameObject.name}");
        for (int i = 0; i < poolSize; i++)
        {
            TrailRenderer trail = Instantiate(prefab, transform);
            trail.gameObject.SetActive(false);
            pool.Enqueue(trail);
        }
    }

    public TrailRenderer GetFromPool(Vector3 position, Quaternion rotation)
    {
        TrailRenderer trail = pool.Dequeue();
        trail.transform.SetPositionAndRotation(position, rotation);
        trail.Clear();
        trail.gameObject.SetActive(true);
        return trail;
    }

    public void ReturnTrail(TrailRenderer trail)
    {
        trail.gameObject.SetActive(false);
        pool.Enqueue(trail);
    }
}