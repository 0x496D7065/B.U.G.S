using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class EnemyAgent : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 7f;
    public int health = 50;
    public int damage = 20;
    public float detectionRadius = 10f;
    public float attackRange = 1f;
    public float attackCd = 1f;
    public bool isDead = false;
    //Event
    public event Action<EnemyAgent> OnDeath;

    //Manual Targeting and Detection logic
    private Transform directTarget;
    private Transform lastSeenPlayer;
    private float lastSeenTime = -999f;
    private float memoryDuration = 5f;
    //FlowField Logic
    public enum TargetType { FlowField, Player }
    public TargetType currentMode = TargetType.FlowField;
    public GridController gridController;
    private EnemyAIManager agentManager;

    public void Init(EnemyAIManager manager)
    {
        gridController = manager.flowField;
        agentManager = manager;
        currentMode = TargetType.FlowField;
        //moveSpeed = UnityEngine.Random.Range((float)6.8, (float)7.2);
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
        Vector3 dir = Vector3.zero;
        if (currentMode == TargetType.Player && directTarget != null)
        {
            dir = (directTarget.position - transform.position).normalized;
        }
        else if (currentMode == TargetType.FlowField && gridController != null)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(transform.position);
            dir = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
        }
        transform.position += moveSpeed * Time.deltaTime * dir.normalized;
    }
    public void TickDetection()
    {
        if (isDead) return;
        Transform nearestPlayer = agentManager.GetNearestPlayerInRange(this, detectionRadius);
        if (nearestPlayer != null)
        {
            lastSeenPlayer = nearestPlayer;
            lastSeenTime = Time.time;
            SetTarget(nearestPlayer);
        }
        else if (Time.time - lastSeenTime < memoryDuration)
            SetTarget(lastSeenPlayer);
        else
            BackToFlowField();
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