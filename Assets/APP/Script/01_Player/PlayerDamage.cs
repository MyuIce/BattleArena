using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// プレイヤー専用ダメージ処理
/// CharaDamageを継承
/// [追加]なし
/// </summary>
public class PlayerDamage : CharaDamage,IDamageable
{
    private Animator anim;
    [SerializeField]GameObject DeathCanvas;
    [SerializeField]Camera DeathCamera;
    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        base.Awake();  // 基底クラスの初期化を呼び出す
        
    }

    

    public override void Death()
    {
        Debug.Log("プレイヤー死亡");
        if (anim != null)
        {
            anim.SetInteger("Death", 1);
        }
        else
        {
            Debug.Log("アニメーション再生失敗");
        }
        //死亡画面
        SoundManager.Instance.OnDeathCanvasSE_Global();
        DeathCanvas.SetActive(true);
        

    }
    public override void DeathEnd()
    {
        DeathCamera.enabled = true;
        this.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("プレイヤー死亡終了");

        

    }

}
