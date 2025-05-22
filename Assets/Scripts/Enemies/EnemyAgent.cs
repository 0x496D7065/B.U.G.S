using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class EnemyAgent : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed;
    public int health;
    public int damage;
    public float detectionRadius;
    public float attackRange;
    public float attackCd;
    public bool isDead = false;
    //Event
    public event Action<EnemyAgent> OnDeath;

    //Manual Targeting and Detection logic
    private Transform directTarget;
    private Transform lastSeenPlayer;
    private float lastSeenTime = -999f;
    private float memoryDuration = 5f;
    //Animation Logic
    private Animator animator;
    private Transform model;
    //Attack Logic
    private float lastAttackTime = -999f;
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
        animator = GetComponentInChildren<Animator>();
        model = transform.Find("Model");
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
            float distToTarget = Vector3.Distance(directTarget.position, transform.position);
            if (distToTarget > attackRange)
            {
                transform.position += moveSpeed * Time.deltaTime * dir.normalized;
            }
            TryInitiateAttack();
        }
        else if (currentMode == TargetType.FlowField && gridController != null)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(transform.position);
            dir = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);
            transform.position += moveSpeed * Time.deltaTime * dir.normalized;
        }
        model.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        // Animation logic
        if (animator != null)
        {
            animator.SetBool("isMoving", dir.magnitude > 0.1f);
        }
    }
    public void TickDetection()
    {
        if (isDead) return;
        Transform nearestPlayer = agentManager.GetNearestVisiblePlayerInRange(this, detectionRadius);
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
    private void TryInitiateAttack()
    {
        Debug.Log($"trying to attack {directTarget.name}");
        if (Time.time - lastAttackTime < attackCd) return;
        if (directTarget == null) return;

        float distance = Vector3.Distance(directTarget.position, transform.position);
        //Debug.Log($"distance= {distance}");
        Debug.Log($"attackRange= {attackRange}");
        if (distance <= attackRange)
            Debug.Log("should be attacking");
        if (distance <= attackRange)
        {
            lastAttackTime = Time.time;
            Debug.Log($"reached the animator trigger");
            animator.SetTrigger("Attack");
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