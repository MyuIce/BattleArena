using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle = 0,      // 待機
    Chase = 1,     // 追跡
    Approach = 2,  // 接近(攻撃準備)
    Attack = 3,    // 攻撃中
    Cooldown = 4   // クールダウン
}

/// <summary>
/// 敵AIの状態管理と行動制御を行うクラス
/// プレイヤーを追跡し、攻撃範囲内で攻撃を実行する
/// </summary>
public class Enemystate1 : MonoBehaviour, IEnemy
{
    [Header("参照")]
    [SerializeField] Charadata charadata;
    [SerializeField] GameObject player;
    
    Animator animator;
    CharacterController characterController;
    

    [Header("距離パラメータ")]
    [SerializeField] float detectionRange = 15f;  // プレイヤー検知範囲
    [SerializeField] float chaseRange = 20f;      // 追跡範囲
    [SerializeField] float attackRange = 2.5f;    // 攻撃範囲
    [SerializeField] float optimalDistance = 2.0f; // 最適距離(未使用)

    [Header("行動パラメータ")]
    [SerializeField] float moveSpeed = 3f;        // 移動速度
    [SerializeField] float rotationSpeed = 5f;    // 回転速度
    [SerializeField] float attackCooldown = 3f;   // 攻撃クールダウン時間
    [SerializeField] float approachTimeout = 2f;  // Approachタイムアウト時間

    [Header("攻撃パラメータ")]
    [SerializeField] int maxComboCount = 4;        // 最大コンボ数
    [SerializeField] float comboInterval = 0.5f;   // 攻撃間の間隔
    [SerializeField] float[] startAttackIndexes = new float[] { 1f }; // 初撃のパターン(例: 1=通常, 5=強攻撃)

    [Header("デバッグ")]
    [SerializeField] bool showDebugGizmos = true; // デバッグ用Gizmosの表示

    private EnemyState currentState = EnemyState.Idle;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;
    private float approachTimer = 0f;  // Approach状態の経過時間

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        
        
        // プレイヤーが未設定の場合、自動で検索
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError($"[{gameObject.name}] Playerが見つかりません。Playerタグを設定してください。");
            }
        }
    }

    void Update()
    {
        // クールダウンタイマーの更新
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;
        
        // AI行動の実行
        EnemyAIaction();
    }

    /// <summary>
    /// 敵AIの行動判断と実行
    /// IEnemyインターフェースの実装
    /// </summary>
    public int EnemyAIaction()
    {
        //プレイヤー死亡時
        if (player == null || !player.activeInHierarchy)
        {
            currentState = EnemyState.Idle;
            if (animator != null)
            {
                animator.SetFloat("Attack", 0f);
                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveZ", 0f);
            }
            return (int)currentState;
        }

        //プレイヤーとの距離
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 状態遷移の判断
        switch (currentState)
        {
            case EnemyState.Idle:
                // 待機状態: プレイヤーが検知範囲内に入ったら追跡開始
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chase;
                    // Chaseへ遷移時のアニメーション
                    animator.SetFloat("Attack", 0f);
                }
                else
                {
                    // Idle時のアニメーション
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);
                }
                break;

            case EnemyState.Chase:
                // 追跡状態: プレイヤーを追いかける
                if (distanceToPlayer > chaseRange)
                {
                    // 追跡範囲外なら待機に戻る
                    currentState = EnemyState.Idle;
                    // Idleへ遷移時のアニメーション
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);
                }
                else if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0)
                {
                    // 攻撃範囲内かつクールダウン完了なら攻撃準備
                    currentState = EnemyState.Approach;
                    approachTimer = 0f;  // タイマーをリセット
                                        
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);
                }
                else
                {                    
                    ChasePlayer();                   
                }
                break;

            case EnemyState.Approach://接近状態                
                approachTimer += Time.deltaTime;
                
                // プレイヤーが攻撃範囲外に出たらChaseに戻る
                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chase;
                    approachTimer = 0f;
                }
                // タイムアウト: 一定時間内に向けなかったらChaseに戻る
                else if (approachTimer > approachTimeout)
                {
                    currentState = EnemyState.Chase;
                    approachTimer = 0f;                                        
                }
                // 30度以内に向いたら攻撃
                else if (IsPlayerInFront(30f))
                {
                    currentState = EnemyState.Attack;
                    approachTimer = 0f;
                    
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);
                }
                else
                {
                    FacePlayer();
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);

                }
                break;

            case EnemyState.Attack:
                // 攻撃状態: 攻撃実行
                Debug.Log($"[{gameObject.name}] Entering Attack state! Setting cooldown to {attackCooldown}s");
                isAttacking = true;
                attackCooldownTimer = attackCooldown;
                
                // Attack時のアニメーション
                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveZ", 0f);

                StartCoroutine(AttackWait());

                currentState = EnemyState.Cooldown;
                break;

            case EnemyState.Cooldown:
                if (attackCooldownTimer <= 0)
                {
                    isAttacking = false;
                    currentState = EnemyState.Chase;
                }
                else
                {
                    animator.SetFloat("MoveX", 0f);
                    animator.SetFloat("MoveZ", 0f);                    
                }
                break;
        }

        // デバッグログ（1秒ごとに表示）
        if (showDebugGizmos && Time.frameCount % 60 == 0)
        {
            float moveX = animator != null ? animator.GetFloat("MoveX") : 0f;
            float moveZ = animator != null ? animator.GetFloat("MoveZ") : 0f;
        }
        return (int)currentState;
    }

    /// <summary>
    /// プレイヤーを追跡する
    /// </summary>
    void ChasePlayer()
    {
        if (player == null || !player.activeInHierarchy) return;
        
        //プレイヤーへの方向
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0; 

        if (characterController != null)
        {
            characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        UpdateMovementAnimation(direction);
        FacePlayer();
    }

    /// <summary>
    /// プレイヤーの方を向く
    /// </summary>
    void FacePlayer()
    {
        if (player == null || !player.activeInHierarchy) return;
        
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0; // Y軸は固定(水平回転のみ)

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// プレイヤーが前方にいるかチェック
    /// </summary>
    /// <param name="angleThreshold">判定角度(度)</param>
    /// <returns>前方にいればtrue</returns>
    bool IsPlayerInFront(float angleThreshold)
    {
        if (player == null || !player.activeInHierarchy) return false;
        
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; // 高さの違いを無視（水平方向のみで判定）
        
        if (directionToPlayer.sqrMagnitude < 0.001f) return true; // ほぼ同じ位置なら正面とみなす

        float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);
        return angle < angleThreshold;
    }

    /// <summary>
    /// 移動アニメーションのパラメータを更新
    /// ワールド座標の移動方向をローカル座標系に変換してBlend Treeに渡す
    /// </summary>
    /// <param name="worldDirection">ワールド座標での移動方向</param>
    void UpdateMovementAnimation(Vector3 worldDirection)
    {
        if (animator == null) return;

        // ワールド座標の移動方向をローカル座標系に変換
        Vector3 localDirection = transform.InverseTransformDirection(worldDirection);

        animator.SetFloat("MoveX", localDirection.x);
        animator.SetFloat("MoveZ", localDirection.z);
    }

    /// <summary>
    /// Gizmosの描画
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // 検知範囲(黄色)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // 追跡範囲(オレンジ)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        // 攻撃範囲(赤)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // プレイヤーへの線
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }

    IEnumerator AttackWait()
    {
        // ランダムな初撃を選択
        float currentAttackIndex = startAttackIndexes[Random.Range(0, startAttackIndexes.Length)];
        //yield return new WaitForSeconds(0.5f);        
        Debug.Log($"[EnemyAttack] Selected Attack Index: {currentAttackIndex}");
        
        // 最大コンボ数までループ
        for (int i = 0; i < maxComboCount; i++)
        {
            // 攻撃アニメーション再生 (Attack=1, 2, 3...)
            // 初撃が5なら 5, 6, 7... と増えていく想定
            animator.SetFloat("Attack", currentAttackIndex + i);
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 0f);

            // 最後の攻撃でない場合、待機(最後の攻撃は硬直を無くす)
            if(i < maxComboCount -1)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (player == null) break;
            // 攻撃範囲外ならコンボ中断
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > attackRange) break;            
        }        
        animator.SetFloat("Attack", 0f);
        currentState = EnemyState.Cooldown;        
    }
}