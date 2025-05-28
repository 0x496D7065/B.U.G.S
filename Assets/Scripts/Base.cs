using UnityEngine;

public class Base : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private HealthUI healthUI;
    [Header("Settings")]
    public int health = 1000;
    public bool isDestroyed = false;
    public ImpactType impactType;

    private void Start()
    {
        healthUI.SetTargetObject(gameObject);
        healthUI.UpdateHealthDisplay(health, "Base");
    }
    public void TakeDamage(int damage, RaycastHit hit)
    {
        if (isDestroyed)
            return;
        ImpactEffectManager.Instance.PlayImpact(impactType, hit, hit.normal);
        health -= damage;
        healthUI.UpdateHealthDisplay(health, "Base");
        if (health <= 0)
            Die();
    }
    public void Die()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        //OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}
