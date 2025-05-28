using UnityEngine;

public class CanBeShot : MonoBehaviour, IDamageable
{
    public ImpactType impactType;
    public void TakeDamage(int damage, RaycastHit hit)
    {
        ImpactEffectManager.Instance.PlayImpact(impactType, hit, hit.normal);
    }
}
