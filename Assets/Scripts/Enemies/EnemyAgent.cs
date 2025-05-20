using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class EnemyAgent : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public int health = 50;
    public bool isDead = false;
    public event Action<EnemyAgent> OnDeath;

    private Transform directTarget; // manual targeting
    public enum TargetType { FlowField, Player }
    public TargetType currentMode = TargetType.FlowField;
    public GridController gridController;

    [SerializeField] private LayerMask enemyLayerMask;
    public void Init(GridController field)
    {
        gridController = field;
        currentMode = TargetType.FlowField;
    }

    public void SetTarget(Transform target)
    {
        directTarget = target;
        currentMode = TargetType.Player;
    }

    public void BackToFlowField()
    {
        directTarget = null;
        currentMode = TargetType.FlowField;
    }

    private void Update()
    {
        if (currentMode == TargetType.Player && directTarget != null)
        {
            Vector3 dir = (directTarget.position - transform.position).normalized;
            transform.position += moveSpeed * Time.deltaTime * dir;
        }
        else if (currentMode == TargetType.FlowField && gridController != null)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(transform.position);
            Vector3 dir = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            transform.position += moveSpeed * Time.deltaTime * dir;
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}