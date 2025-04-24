using UnityEngine;

public class CasingEject : MonoBehaviour
{
    public float ejectForce = 1f;
    public float upwardForce = 1f;
    public float gravity = -9.8f;
    private Vector3 velocity;
    private Vector3 spinAxis;
    private float lifeTimer;
    public float lifeTime = 2f;
    public bool InUse { get; private set; }

    void OnEnable()
    {
        InUse = true;
        lifeTimer = lifeTime;
        Invoke(nameof(Despawn), lifeTime);
        transform.localRotation = Quaternion.identity;
        // Eject direction (sideways + up + a bit of randomness)
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        Vector3 randomOffset = new(
            Random.Range(-0.2f, 0.2f),
            Random.Range(0f, 0.2f),
            Random.Range(-0.2f, 0.2f)
        );
        spinAxis = Random.onUnitSphere * 180f;

        velocity = right * ejectForce + up * upwardForce + randomOffset;
    }

    void Update()
    {
        velocity += gravity * Time.deltaTime * Vector3.up;
        transform.position += velocity * Time.deltaTime;
        transform.Rotate(spinAxis * Time.deltaTime);
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
    private void Despawn()
    {
        InUse = false;
        gameObject.SetActive(false);
    }
}
