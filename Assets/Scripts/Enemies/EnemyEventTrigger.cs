using UnityEngine;

public class EnemyEventTrigger : MonoBehaviour
{
    public EnemyAgent Agent;

    private void Awake()
    {
        Agent = GetComponentInParent<EnemyAgent>();
    }
    public void TryAttack()
    {
        if (Agent != null)
            Agent.TryHit();
    }
}
