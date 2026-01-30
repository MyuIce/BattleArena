using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    //装備込みの合計ステータスを管理するオブジェクト
    [SerializeField] private TotalRuntimeStatus runtimeStatus;
    
    //剣のコライダー（攻撃判定の有効/無効を確認するため）
    private Collider swordCollider;
    
    //この攻撃でヒット済みの敵を記録（同じ攻撃で複数回ダメージを与えないため）
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();
    
    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトのコライダーを取得
        swordCollider = GetComponent<Collider>();
        
        if (swordCollider == null)
        {
            Debug.LogError($"[PlayerAttack] {gameObject.name}にColliderが見つかりません。");
        }
    }
    
    /// <summary>
    /// ヒット済みリストをクリア（新しい攻撃の開始時に呼び出す）
    /// </summary>
    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
        Debug.Log($"[PlayerAttack] Hit list cleared - New attack started");
    }
    
    //ゲームオブジェクトのコライダーの接触時に呼び出す
    void OnTriggerEnter(Collider other)
    {
        //プレイヤー自身への攻撃判定を防ぐ
        if (other.CompareTag("Player")) return;

        //剣のコライダーが無効な場合（攻撃中でない場合）はダメージを与えない
        if (swordCollider == null || !swordCollider.enabled) return;

        //すでにヒット済みの敵には再度ダメージを与えない
        if (hitEnemies.Contains(other)) return;

        //接触したオブジェクトがダメージを受けられるか判定し、処理
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            SoundManager.Instance.OnAttackSE_Global();
            //装備込みの合計ステータスを取得
            var totalStatus = runtimeStatus.GetTotalStatus();
            
            //装備を含めた合計攻撃力でダメージを与える
            damageable.Damage(totalStatus.ATK);
            
            
        }
    }
}
