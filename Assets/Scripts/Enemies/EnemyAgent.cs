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
    private TargetInfo? directTarget;
    private TargetInfo? lastSeenTarget;
    private float lastSeenTime = -999f;
    private float memoryDuration = 5f;
    public struct TargetInfo
    {
        public Transform targetTransform;
        public Vector3 closestPoint;
    }
    //Animation Logic
    private Animator animator;
    private Transform model;
    //Attack Logic
    private float lastAttackTime = -999f;
    private LayerMask targetMask;
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
        targetMask = LayerMask.GetMask("Player", "Base", "Construction");
        //moveSpeed = UnityEngine.Random.Range((float)6.8, (float)7.2);
    }

    public void SetTarget(TargetInfo? target)
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
            dir = (directTarget.Value.targetTransform.position - transform.position).normalized;
            float distToTarget = Vector3.Distance(directTarget.Value.closestPoint, transform.position);
            if (distToTarget > attackRange)
            {
                transform.position += moveSpeed * Time.deltaTime * dir.normalized;
            }
            TryInitiateAttack(distToTarget);
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
        TargetInfo? nearestTarget = agentManager.GetNearestVisibleTargetInRange(this, detectionRadius);
        if (nearestTarget != null)
        {
            lastSeenTarget = nearestTarget;
            lastSeenTime = Time.time;
            SetTarget(nearestTarget);
        }
        else if (Time.time - lastSeenTime < memoryDuration)
            SetTarget(lastSeenTarget);
        else
            BackToFlowField();
    }
    private void TryInitiateAttack(float distToTarget)
    {
        //Debug.Log($"trying to attack {directTarget.Value.targetTransform.name}");
        if (Time.time - lastAttackTime < attackCd) return;
        if (directTarget == null) return;

        //float distance = Vector3.Distance(directTarget.Value.closestPoint, transform.position);
        //Debug.Log($"distance= {distance}");
        //Debug.Log($"attackRange= {attackRange}");
        //if (distToTarget <= attackRange)
            //Debug.Log("should be attacking");
        if (distToTarget <= attackRange)
        {
            lastAttackTime = Time.time;
            //Debug.Log($"reached the animator trigger");
            animator.SetTrigger("Attack");
        }
    }
    public void TryHit()
    {
        Vector3 origin = transform.position + (Vector3.up * (float)0.5f);
        Vector3 direction = model.forward;
        //Debug.Log("Casting to hit");
        //Debug.DrawRay(origin, direction * attackRange, Color.red, 1.0f);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, targetMask))
        {
            //Debug.Log("ray has hit");
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                //Debug.Log("target hit, sending dmg");
                damageable.TakeDamage(damage);
            }
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