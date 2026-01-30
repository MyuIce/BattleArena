using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum EnemyState
{
    Idle,
    Chase,
    Approach,
    Attack,
    Cooldown
}
*/

/// <summary>
/// 敵AIの状態管理と行動制御
/// </summary>
public class EnemyStateController : MonoBehaviour, IEnemy
{
    [Header("References")]
    [SerializeField] private Charadata charadata;
    [SerializeField] private GameObject player;

    private Animator animator;
    private CharacterController controller;

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float chaseRange = 20f;
    [SerializeField] private float attackRange = 2.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float approachTimeout = 2f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private EnemyState currentState = EnemyState.Idle;
    private float cooldownTimer;
    private float approachTimer;
    private bool isAttacking;

    #region Unity Lifecycle
    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        UpdateCooldown();
        UpdateState();
    }
    #endregion

    #region State Machine
    public int EnemyAIaction()
    {
        UpdateState();
        return (int)currentState;
    }

    private void UpdateState()
    {
        if (!IsPlayerAlive())
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float distance = DistanceToPlayer();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distance <= detectionRange)
                    ChangeState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                HandleChase(distance);
                break;

            case EnemyState.Approach:
                HandleApproach(distance);
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;

            case EnemyState.Cooldown:
                if (cooldownTimer <= 0)
                    ChangeState(EnemyState.Chase);
                break;
        }
    }

    private void ChangeState(EnemyState next)
    {
        if (currentState == next) return;

        currentState = next;
        approachTimer = 0f;

        switch (currentState)
        {
            case EnemyState.Idle:
                ResetAnimation();
                break;

            case EnemyState.Attack:
                if (!isAttacking)
                    StartCoroutine(AttackRoutine());
                break;
        }
    }
    #endregion

    #region State Handlers
    private void HandleChase(float distance)
    {
        if (distance > chaseRange)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        if (distance <= attackRange && cooldownTimer <= 0)
        {
            ChangeState(EnemyState.Approach);
            return;
        }

        ChasePlayer();
    }

    private void HandleApproach(float distance)
    {
        approachTimer += Time.deltaTime;

        if (distance > attackRange || approachTimer > approachTimeout)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        if (IsPlayerInFront(30f))
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        FacePlayer();
        ResetAnimation();
    }

    private void HandleAttack()
    {
        // Coroutine側で制御
    }
    #endregion

    #region Movement
    private void ChasePlayer()
    {
        Vector3 dir = DirectionToPlayer();
        Move(dir);
        UpdateMoveAnimation(dir);
        FacePlayer();
    }

    private void Move(Vector3 direction)
    {
        if (controller != null)
            controller.Move(direction * moveSpeed * Time.deltaTime);
        else
            transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void FacePlayer()
    {
        Vector3 dir = DirectionToPlayer();
        if (dir == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }
    #endregion

    #region Attack
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        cooldownTimer = attackCooldown;

        animator.SetFloat("Attack", 1f);
        yield return new WaitForSeconds(0.5f);

        if (DistanceToPlayer() <= attackRange)
        {
            animator.SetFloat("Attack", 2f);
            yield return new WaitForSeconds(0.5f);
        }

        animator.SetFloat("Attack", 0f);
        isAttacking = false;
        ChangeState(EnemyState.Cooldown);
    }
    #endregion

    #region Utility
    private void UpdateCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private bool IsPlayerAlive()
    {
        return player != null && player.activeInHierarchy;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private Vector3 DirectionToPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0;
        return dir.normalized;
    }

    private bool IsPlayerInFront(float angle)
    {
        Vector3 dir = DirectionToPlayer();
        return Vector3.Angle(transform.forward, dir) < angle;
    }

    private void UpdateMoveAnimation(Vector3 worldDir)
    {
        Vector3 local = transform.InverseTransformDirection(worldDir);
        animator.SetFloat("MoveX", local.x);
        animator.SetFloat("MoveZ", local.z);
    }

    private void ResetAnimation()
    {
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        animator.SetFloat("Attack", 0);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    #endregion
}
