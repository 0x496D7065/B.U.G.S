using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;

    private float lifeTimer;
    public float lifeTime = 5f;
    public int damage = 20;
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
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("enemy"))
            hitObject.GetComponent<EnemyAgent>().TakeDamage(damage);
        Debug.Log($"Bullet hit {collision.gameObject.name}");
        gameObject.SetActive(false); // return to pool
    }
}