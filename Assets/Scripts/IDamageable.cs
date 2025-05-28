using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, RaycastHit hit);
}
