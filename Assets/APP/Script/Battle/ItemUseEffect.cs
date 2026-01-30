using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム使用時の効果を適用するクラス
/// アイテムの効果(HP回復、バフなど)をプレイヤーに適用する責任を持つ
/// </summary>
public class ItemUseEffect : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private CharaDamage charaDamage;  // HP管理用
    [SerializeField] private StatusCalc statusCalc;    // ステータスバフ管理用

    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private GameObject defenceEffectPrefab;
    

    private void Awake()
    {
        // 自動で取得
        if (charaDamage == null)
        {
            charaDamage = GetComponent<CharaDamage>();
        }

        // StatusCalcはシーン内に1つだけ存在する想定
        if (statusCalc == null)
        {
            statusCalc = FindObjectOfType<StatusCalc>();
        }
    }

    /// <summary>
    /// HP回復効果
    /// </summary>
    public void Heal(int amount)
    {
        if (charaDamage == null)
        {
            Debug.LogError("[ItemUseEffect] CharaDamageが設定されていません");
            return;
        }

        charaDamage.Heal(amount);
        Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
        Debug.Log($"[ItemUseEffect] HP回復: +{amount}");
    }

    /// <summary>
    /// 攻撃力バフ効果
    /// </summary>
    public void AddAttackBuff(int amount, float duration)
    {
        if (statusCalc == null)
        {
            Debug.LogError("[ItemUseEffect] StatusCalcが見つかりません");
            return;
        }

        StatusData buff = new StatusData { ATK = amount };
        statusCalc.AddItemBuff("ATK Buff", buff, duration);
        Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
        Debug.Log($"[ItemUseEffect] 攻撃力バフ: +{amount} ({duration}秒間)");
    }

    /// <summary>
    /// 防御力バフ効果
    /// </summary>
    public void AddDefenseBuff(int amount, float duration)
    {
        if (statusCalc == null)
        {
            Debug.LogError("[ItemUseEffect] StatusCalcが見つかりません");
            return;
        }

        StatusData buff = new StatusData { DEF = amount };
        statusCalc.AddItemBuff("DEF Buff", buff, duration);
        Instantiate(defenceEffectPrefab, transform.position, Quaternion.identity);
        Debug.Log($"[ItemUseEffect] 防御力バフ: +{amount} ({duration}秒間)");
    }

    /// <summary>
    /// 素早さバフ効果
    /// </summary>
    public void AddSpeedBuff(int amount, float duration)
    {
        if (statusCalc == null)
        {
            Debug.LogError("[ItemUseEffect] StatusCalcが見つかりません");
            return;
        }

        StatusData buff = new StatusData { AGI = amount };
        statusCalc.AddItemBuff("AGI Buff", buff, duration);
        Debug.Log($"[ItemUseEffect] 素早さバフ: +{amount} ({duration}秒間)");
    }

    /// <summary>
    /// 知力バフ効果
    /// </summary>
    public void AddIntelligenceBuff(int amount, float duration)
    {
        if (statusCalc == null)
        {
            Debug.LogError("[ItemUseEffect] StatusCalcが見つかりません");
            return;
        }

        StatusData buff = new StatusData { INT = amount };
        statusCalc.AddItemBuff("INT Buff", buff, duration);
        Debug.Log($"[ItemUseEffect] 知力バフ: +{amount} ({duration}秒間)");
    }

    /// <summary>
    /// 魔法防御バフ効果
    /// </summary>
    public void AddResistanceBuff(int amount, float duration)
    {
        if (statusCalc == null)
        {
            Debug.LogError("[ItemUseEffect] StatusCalcが見つかりません");
            return;
        }

        StatusData buff = new StatusData { RES = amount };
        statusCalc.AddItemBuff("RES Buff", buff, duration);
        Debug.Log($"[ItemUseEffect] 魔法防御バフ: +{amount} ({duration}秒間)");
    }

    /// <summary>
    /// 複合効果(複数のステータスを同時にバフ)
    /// </summary>
    public void ApplyMultipleEffects(int healAmount, StatusData buffAmount, float duration)
    {
        // HP回復
        if (healAmount > 0)
        {
            Heal(healAmount);
        }

        // ステータスバフ
        if (statusCalc != null)
        {
            statusCalc.AddItemBuff("Multi Buff", buffAmount, duration);
            Debug.Log($"[ItemUseEffect] 複合バフ適用: ATK+{buffAmount.ATK}, DEF+{buffAmount.DEF}, AGI+{buffAmount.AGI}");
        }
    }

    /// <summary>
    /// 全てのバフをクリア(デバッグ用)
    /// </summary>
    public void ClearAllBuffs()
    {
        if (statusCalc != null)
        {
            statusCalc.ClearAllItemBuffs();
            Debug.Log("[ItemUseEffect] 全てのバフをクリアしました");
        }
    }
}
