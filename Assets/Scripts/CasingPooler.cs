using System.Collections.Generic;
using UnityEngine;

public class CasingPooler : MonoBehaviour
{
    public GameObject casingPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject casing = Instantiate(casingPrefab);
            casing.SetActive(false);
            pool.Enqueue(casing);
        }
    }

    public GameObject GetCasing(Vector3 position, Quaternion rotation)
    {
        GameObject casingToUse = null;

        // Try to find an inactive casing first
        foreach (GameObject casing in pool)
        {
            var fake = casing.GetComponent<CasingEject>();
            if (fake != null && !fake.InUse)
            {
                casingToUse = casing;
                break;
            }
        }

        // If all casings are active, reuse the oldest
        if (casingToUse == null)
        {
            casingToUse = pool.Dequeue();
            casingToUse.SetActive(false);
        }
        else
        {
            // Remove it from the queue to reinsert it at the end
            pool = new Queue<GameObject>(pool); // refresh queue order
            pool = new Queue<GameObject>(RemoveFirst(pool, casingToUse));
        }

        // Prepare casing
        casingToUse.transform.SetPositionAndRotation(position, rotation);
        casingToUse.SetActive(true);

        // Move to end of the queue
        pool.Enqueue(casingToUse);

        return casingToUse;
    }

    private IEnumerable<GameObject> RemoveFirst(Queue<GameObject> q, GameObject target)
    {
        foreach (var obj in q)
        {
            if (obj != target)
                yield return obj;
        }
    }
}
