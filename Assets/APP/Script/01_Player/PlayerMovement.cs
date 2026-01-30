using System.Collections;
using System.Collections.Generic;//List<T>,Dictionary<TKey,TValue>,Queue<T>,Stack<T>が使える
using System.Threading;//スレッド処理を使えるようになるがCoroutineをつかうことが多い
using UnityEngine;//MonoBehaviour,GameObject,Transform,Vector3,Timeなど
using UnityEngine.EventSystems;//EventSystemなど


//==================================================================
//キャラクターの移動、アニメーション（移動、攻撃モーション）
//カメラの処理(向きの処理やカメラに合わせたキャラクターの回転処理)
//==================================================================
//リファクタリング案→移動処理や攻撃アニメーションなどの処理をUpdate()に書かずに別の関数に記述してUpdate()で呼び出す

public class PlayerMovement : MonoBehaviour
{
    [Header("モデル参照")]
    [SerializeField] private GameObject playerModel;

    //=====コンポーネント=====
    private Animator animator; // Animatorのコンポーネント
    private Rigidbody rb;
    private Transform cam;//カメラの位置
    public Canvas DeathCanvas;

    
    //=====移動関連=====
    [Header("移動関連")]
    public float speed = 1f;
    public float rotationSpeed = 1f;
    float moveZ; // 前後移動量
    float moveY; // 上下移動量
    float moveX; // 左右移動量
    float mouseX;   // マウス移動量X
    float mouseY;   // マウス移動量Y
    Vector3 moveDirection = Vector3.zero;//通常移動方向
    float normalSpeed = 4f; // 通常の移動速度

    //=====カメラ関連=====
    Vector3 startPosition;   // 開始位置
    Vector3 currentRotation;   // 現在の回転
    Vector3 originalRotation; // 回転記憶
    Vector3 raycalc; //光計算
    
    //=====攻撃関連=====
    [Header("攻撃関連")]
    private const int MAX_COMBO_COUNT = 4; //最大コンボ段数
    float Attack; //攻撃段階 (0:待機, 1-4:攻撃)
    bool Combo = false; //コンボ許容
    bool Attack_Possible = true; //攻撃可能
    float ComboEndtime = 1f; //コンボ終了時のクールタイム
    [SerializeField] private Collider swordCollider;//剣の当たり判定

    [Header("コンポーネント参照")]
    private CharaDamage CharaDamage;



    void Start()
    {
        if (playerModel == null)
        {
            playerModel = GetComponentInChildren<Animator>()?.gameObject;
            if (playerModel == null)
            {
                Debug.LogError("PlayerModelがInspectorで設定されておらず、自動検出も失敗しました。");
                return;
            }
        }
        animator = playerModel.GetComponent<Animator>();
        rb = playerModel.GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        
        CharaDamage = GetComponentInChildren<CharaDamage>();
        // マウスカーソルを非表示にし、位置を固定
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        // 開始位置を記録
        startPosition = transform.position;
        
        // DeathCanvasが割り当てられている場合のみ無効化
        if (DeathCanvas != null)
        {
            DeathCanvas.enabled = false;
        }
    }

    void Update()
    {
        //移動処理
        HandleInput();
        //カメラ処理
        HandleCamera();
        //攻撃処理
        HandleAttack();
        //ガード処理
        HandleGuard();
        

        raycalc = playerModel.transform.position;
        raycalc.y += 0.5f;
    }



    void HandleInput()
    {
        // 前後左右の入力を取得
        moveZ = Input.GetAxis("Vertical");
        moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;
        playerModel.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);

        // アニメーションの再生方向を判定
        float absMoveZ = Mathf.Abs(moveDirection.z);
        float absMoveX = Mathf.Abs(moveDirection.x);

        // アニメーターに移動速度を渡す
        animator.SetFloat("MoveSpeed", moveDirection.magnitude);
    }

    

    void HandleCamera()
    {
        //カメラの向いている方向に合わせてプレイヤーを移動させる
        Vector3 movePlayer = new Vector3(moveZ, 0, moveX).normalized;
        if (movePlayer != Vector3.zero)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * movePlayer.z + camRight * movePlayer.x;

            rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.deltaTime);
            //プレイヤーがカメラ方向を向くように回転
            Quaternion targetRot = Quaternion.LookRotation(camForward);
            playerModel.transform.rotation = targetRot;
        }
    }

    void HandleAttack()
    {
        // 攻撃可能状態でない場合は早期リターン
        if (!Attack_Possible) return;
        
        // 現在の攻撃段階を取得
        Attack = animator.GetFloat("Attack");

        // コンボ中の処理
        if (Combo)
        {
            // 4段目に到達したらリセット
            if (Attack >= MAX_COMBO_COUNT)
            {
                animator.SetFloat("Attack", 0f);
                Attack_Possible = false;
                StartCoroutine(ComboEnd());
                return;
            }
            
            // 左クリックで次の攻撃段階へ
            if (Input.GetMouseButtonDown(0))
            {
                Combo = false;
                Attack += 1f;
                animator.SetFloat("Attack", Attack);
            }
        }
        // 待機状態からの初撃
        else if (Attack == 0 && Input.GetMouseButtonDown(0))
        {
            animator.SetFloat("Attack", 1f);
        }
    }

    void HandleGuard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 方法1: 即座にガードアニメーションを再生
            animator.Play("Guard", 0, 0f);  // レイヤー0、開始位置0%

            // 攻撃をキャンセル
            Attack = 0;
            animator.SetFloat("Attack", 0f);
            Attack_Possible = false;
            Combo = false;
            
            // 剣の当たり判定を無効化
            if (swordCollider != null)
            {
                swordCollider.enabled = false;
            }
        }
    }




    

    IEnumerator ComboEnd()
    {
        //コンボ終了待機
        yield return new WaitForSeconds(ComboEndtime);
        Attack_Possible = true;
    }
    
    public void OnHitFromAnimation()//攻撃ヒット処理(コンボカウンターオン&&剣の当たり判定出現)
    {
        Combo = true;
        Debug.Log("[DEBUG] AnimationEvent: Hit() を受け取りました");
        swordCollider.enabled = true;
    }

    public void OnAttackEndFromAnimation()//攻撃終了処理(コンボカウンターオフ&&剣の当たり判定消失)
    {
        Combo = false;
        swordCollider.enabled = false;
        animator = playerModel.GetComponent<Animator>();
        animator.SetFloat("Attack",0f);
        StartCoroutine(ComboEnd());
    }

    public void OnGuardEnd()
    {
        Attack_Possible = true;    
    }
    
    public void ParryTime()
    {
        // ガードの「盾を構えた瞬間」に無敵開始
        if (CharaDamage != null)
        {
            CharaDamage.SetInvincible(30);
            Debug.Log("[PlayerMovement] ガード無敵開始: 30フレーム");
        }
        else
        {
            Debug.LogError("[PlayerMovement] CharaDamageコンポーネントが見つかりません！");
        }
    }
}

