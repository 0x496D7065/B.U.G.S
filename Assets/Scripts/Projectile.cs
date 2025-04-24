using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;

    private float lifeTimer;
    public float lifeTime = 5f;
    void OnEnable()
    {
        lifeTimer = lifeTime;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            gameObject.SetActive(false); // Return to pool
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optional: Check tag/layer here
        gameObject.SetActive(false); // return to pool
    }
}