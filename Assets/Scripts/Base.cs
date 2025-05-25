using UnityEngine;

public class Base : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private HealthUI healthUI;
    [Header("Settings")]
    public int health = 1000;
    public bool isDestroyed = false;

    private void Start()
    {
        healthUI.SetTargetObject(gameObject);
        healthUI.UpdateHealthDisplay(health, "Base");
    }
    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;
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
