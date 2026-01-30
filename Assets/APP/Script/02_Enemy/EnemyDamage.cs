using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵専用ダメージ処理
/// CharaDamageを継承
/// 宝箱ドロップ
/// </summary>
public class EnemyDamage : CharaDamage
{
    [SerializeField] private GameObject treasureBoxPrefab;
    [SerializeField] private Transform dropPoint;

    private Animator anim;
    public enum EnemyType
    {
        Skeleton,
        Zombie
    }
    [SerializeField] public EnemyType enemyType;

    

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public override void Death()
    {
        //enumに合わせた死亡音
        switch(enemyType)
        {
            case EnemyType.Skeleton:
                SoundManager.Instance.OnSkeletonDownSE_Global();
                break;
            case EnemyType.Zombie:
                SoundManager.Instance.OnZombieDownSE_Global();
                break;
        }
        SpawnTreasure();
        // アニメーション再生
        if (anim != null)
        {
            anim.SetInteger("Death", 1);
        }
        else
        {
            Debug.Log("アニメーション再生失敗");
        }
    }

    /// <summary>
    /// 宝箱生成
    /// </summary>
    private void SpawnTreasure()
    {
        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position;

        Quaternion spawnRotation =  Quaternion.Euler(0, 180, 0);
        Instantiate(treasureBoxPrefab, spawnPos, spawnRotation);
        Debug.Log("宝箱生成");
    }

    
}
