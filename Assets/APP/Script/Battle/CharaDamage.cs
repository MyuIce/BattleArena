using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// キャラクターがダメージを受けるときの「基底クラス」
/// キャラクターのダメージ処理とHP管理を行うクラス
/// IDamageableインターフェースを実装し、攻撃を受けられるようにする
/// </summary>
public class CharaDamage : MonoBehaviour, IDamageable
{
    [Header("キャラクターデータ")]
    [SerializeField] protected Charadata charadata;  
    [SerializeField] protected Slider Slider;    
       

    [Header("ダメージUI")]
    [SerializeField] protected DamageTextManager damageTextManager; 

    [Header("キャラのステータス")]
    public int HP;
    public int MAXHP;
    protected int ATK;
    protected int DEF;
    protected int INT;
    protected int RES;
    protected int AGI;
    protected int EXP;

    [Header("プレイヤー専用設定")]
    [SerializeField] private TotalRuntimeStatus runtimeStatus;


    //無敵フラグ
    protected bool invincible = false;
    protected int invincibleTime = 0;

    //死亡フラグ
    protected bool isDead = false;

    //================================================
    // Editor用プロパティ
    //================================================
    public bool isInvincible
    {
        get { return invincible; }
        set { invincible = value; }
    }

    
    
    protected virtual void Awake()
    {
        IsInitializeStatus();
    }

    /// <summary>
    /// ステータスの初期化処理（キャラの基礎ステータスの参照）
    /// </summary>
    protected virtual void IsInitializeStatus()
    {
        //ステータスの取得
        var Status = charadata.GetCharaStatus();
        var Raw = charadata.GetRawStatus();

        if(charadata != null)
        {
            //HPゲージのスライダーを最大値(1.0)に設定
            Slider.value = 1;

            //Charadataから最大HPとステータスを初期化
            HP = Raw.MAXHP;
            MAXHP = Raw.MAXHP;
            ATK = Status.ATK;
            DEF = Status.DEF;
            INT = Status.INT;
            RES = Status.RES;
            AGI = Status.AGI;
            
        }
    }

    /// <summary>
    /// ダメージ処理を受けたときの処理
    /// virtualで定義して派生クラスで実装
    /// </summary>
    /// <param name="value"></param>
    public virtual void Damage(int value)
    {
        //死亡状態チェック
        if(isDead) return;

        // 無敵状態チェック
        if (invincible && Time.frameCount < invincibleTime)
        {
            int remainingFrames = invincibleTime - Time.frameCount;
            SoundManager.Instance.OnGuardSE_Global();
            
            
            // JustParry成功時のテキスト表示
            if (damageTextManager != null)
            {
                damageTextManager.ShowJustParry(transform.position + Vector3.up * 2f);
            }
            return;
        }
        // 無敵時間が終了していたらフラグをリセット
        if (Time.frameCount >= invincibleTime)
        {
            invincible = false;
        }

        var Raw = charadata.GetRawStatus();
        int defenseValue;

        if(runtimeStatus != null)
        {
            var totalStatus = runtimeStatus.GetTotalStatus();
            defenseValue = totalStatus.DEF;
        }
        else
        {
            var Status = charadata.GetCharaStatus();
            defenseValue = Status.DEF;
        }
        int actualDamage = value - defenseValue;
        HP -= actualDamage;
            
        // ダメージテキストを表示(派生クラスで表示方法を変更可能)
        OnDamageReceived(actualDamage);
        
        // HPゲージのスライダーを更新(現在HP / 最大HP)
        Slider.value = (float)HP / (float)Raw.MAXHP;

        // HPが0以下になったら死亡処理を実行
        if (HP <= 0)
        {
            isDead = true;
            Death();
        }
    }

    ///<summary>
    /// ダメージを受けた際のフィードバック
    /// ダメージテキストを表示
    /// <summary>
    protected virtual void OnDamageReceived(int damage)
    {
        Debug.Log($"[CharaDamage] 受けたダメージ: {damage}");
        
        // ダメージテキストを表示
        if (damageTextManager != null)
        {
            // HPバーの位置を基準にダメージテキストを表示
            Vector3 damageTextPosition = transform.position + Vector3.up * 2f; // キャラの頭上2m
            damageTextManager.ShowDamage(damage, damageTextPosition);
        }
    }

    public void SetInvincible(float time)
    {
        invincible = true;
        invincibleTime = Time.frameCount + (int)time;
        Debug.Log($"[CharaDamage] 無敵開始: {(int)time}フレーム (現在: {Time.frameCount}, 終了: {invincibleTime})");
    }

    public virtual void Death()
    {
        
    }
    public virtual void DeathEnd()
    {
        this.gameObject.SetActive(false);
    }



    
    //================================================
    // HP回復メソッド(ItemUseEffectから呼び出される)
    //================================================

    /// <summary>
    /// HP回復
    /// ItemUseEffectから呼び出される
    /// </summary>
    public void Heal(int amount)
    {   
        HP += amount;
        if (HP > MAXHP) HP = MAXHP;

        // HPゲージ更新
        if (Slider != null)
        {
            Slider.value = (float)HP / (float)MAXHP;
        }

    }

    //================================================
    // Editor用HP調整
    //================================================
    public void SetHP(int hp)
    {
        // まずHPを設定
        HP = Mathf.Clamp(hp, 0, MAXHP);
        
        // その後、Sliderを更新
        if (Slider != null)
        {
            Slider.value = (float)HP / (float)MAXHP;
        }
    }
}
