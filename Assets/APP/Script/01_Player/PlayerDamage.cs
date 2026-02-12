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

    [SerializeField] private Renderer characterRender; // プレイヤーのRenderer
    [SerializeField] private Material flashMaterial;   // ダメージ時に差し替えるマテリアル
    [SerializeField] float flashDuration = 0.2f;      // 差し替える時間
    
    private Material[] originalMaterials;             // 元のマテリアル配列を保存用

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        base.Awake();  // 基底クラスの初期化を呼び出す
        
        if(characterRender != null)
        {
            // 元のマテリアルを保存
            originalMaterials = characterRender.materials;
        }
    }

    //ダメージを受けたときの処理
    protected override void OnDamageReceived(int damage)
    {
        base.OnDamageReceived(damage); // 元のダメージテキスト表示などを実行
        
        // マテリアル差し替えコルーチンを開始
        if (characterRender != null && flashMaterial != null)
        {
            StopCoroutine(nameof(FlashRoutine));
            StartCoroutine(nameof(FlashRoutine));
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (originalMaterials == null || originalMaterials.Length == 0) yield break;

        // ヒットフラッシュ用マテリアルに差し替え
        // ※Flashマテリアルに元のテクスチャを反映（マテリアルの第1スロットを想定）
        flashMaterial.SetTexture("_MainTex", originalMaterials[0].mainTexture);
        
        // 全てのスロットをフラッシュ用マテリアルにする
        Material[] flashMaterials = new Material[originalMaterials.Length];
        for (int i = 0; i < flashMaterials.Length; i++)
        {
            flashMaterials[i] = flashMaterial;
        }
        characterRender.materials = flashMaterials;
        
        // シェーダーのプロパティを制御
        flashMaterial.SetFloat("_FlashAmount", 1.0f);
        
        yield return new WaitForSeconds(flashDuration);
        
        // 元のマテリアルに戻す
        characterRender.materials = originalMaterials;
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
