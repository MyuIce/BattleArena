using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装備と基礎ステータス、アイテムバフの計算を行うクラス
/// (装備変更時、アイテム使用時などのTotalStatusを計算してTotalRuntimeStatusに渡す)
/// </summary>
public class StatusCalc : MonoBehaviour
{
    [SerializeField] private Soubikanri soubiManager; //装備管理オブジェクト
    [SerializeField] private Charadata charaData;      // キャラデータ(ScriptableObject)
    [SerializeField] public CharaStatuskanri statusUI; // ステータスUI
    [SerializeField] private TotalRuntimeStatus runtimeStatus; // TotalRuntimeStatus

    // 装備の合計ステータス
    private StatusData equipSum;
    
    // アイテムバフの合計ステータス
    private StatusData itemBuffSum;

    // アクティブなバフのリスト
    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    /// <summary>
    /// アクティブなバフ情報
    /// </summary>
    private class ActiveBuff
    {
        public string name;
        public StatusData buffValue;
        public float endTime;
    }

    void Start()
    {
        equipSum = new StatusData();
        itemBuffSum = new StatusData();
        CalculateTotalStatus();
    }

    /// <summary>
    /// 最終ステータスを計算
    /// キャラ基礎 + 装備 + アイテムバフ = 最終ステータス
    /// </summary>
    public void CalculateTotalStatus()
    {
        // キャラデータ(ATK,DEF,INT,AGI,RES)の取得と定義
        var CharaStatus = charaData.GetCharaStatus();

        // 装備中の装備を取得
        var equippedItems = soubiManager.GetEquippedItems(); 

        // 装備中の装備のステータスを合計
        equipSum = new StatusData();

        foreach (var eq in equippedItems.Values)
        {
            if (eq == null)
            {
                Debug.Log("[DEBUG] 装備が null です");
                continue;
            }
            else
            {
                Debug.Log("[DEBUG] 装備が null ではありません");
            }
            var s = eq.GetItemStatus();
            equipSum.ATK += s.ATK;
            equipSum.DEF += s.DEF;
            equipSum.AGI += s.AGI;
            equipSum.INT += s.INT;
            equipSum.RES += s.RES;

            Debug.Log($"[DEBUG] 装備合計: ATK={equipSum.ATK}, DEF={equipSum.DEF}, AGI={equipSum.AGI}, INT={equipSum.INT}, RES={equipSum.RES}");
            Debug.Log($"[DEBUG] {eq.GetItemname()} の装備ステータス: ATK={s.ATK}, DEF={s.DEF}, AGI={s.AGI}, INT={s.INT}, RES={s.RES}");
        }

        
        //UI更新
        if (statusUI != null)
        {
            statusUI.UpdateStatusDisplay();
            Debug.Log("[DEBUG] UIが更新されました");
        }
        else
        {
            Debug.LogWarning("[WARNING] statusUI が設定されていません");
        }

        // 最終ステータス = キャラ基礎 + 装備 + アイテムバフ
        StatusData TotalStatus = CharaStatus + equipSum + itemBuffSum;
        
        // TotalRuntimeStatusに最終結果を設定
        runtimeStatus.SetTotalStatus(TotalStatus);
        Debug.Log($"[StatusCalc] TotalStatusが更新されました (キャラ + 装備 + アイテムバフ)");
    }

    /// <summary>
    /// アイテムバフを追加(時限式)
    /// </summary>
    public void AddItemBuff(string buffName, StatusData buff, float duration)
    {
        // バフを追加
        itemBuffSum += buff;
        
        // アクティブバフリストに追加
        activeBuffs.Add(new ActiveBuff
        {
            name = buffName,
            buffValue = buff,
            endTime = Time.time + duration
        });

        // 最終ステータスを再計算
        CalculateTotalStatus();

        Debug.Log($"[StatusCalc] Item Buff Added: {buffName} " +
                  $"(ATK+{buff.ATK}, DEF+{buff.DEF}, AGI+{buff.AGI}, INT+{buff.INT}, RES+{buff.RES}) " +
                  $"Duration: {duration}s");

        // 時間経過後にバフを削除
        StartCoroutine(RemoveBuffAfterDuration(buffName, buff, duration));
    }

    /// <summary>
    /// 指定時間後にバフを削除
    /// </summary>
    private IEnumerator RemoveBuffAfterDuration(string buffName, StatusData buff, float duration)
    {
        yield return new WaitForSeconds(duration);

        // バフを削除
        itemBuffSum -= buff;
        
        // アクティブバフリストから削除
        activeBuffs.RemoveAll(b => b.name == buffName && Time.time >= b.endTime);

        // 最終ステータスを再計算
        CalculateTotalStatus();

        Debug.Log($"[StatusCalc] Item Buff Expired: {buffName}");
    }

    /// <summary>
    /// 全てのアイテムバフをクリア
    /// </summary>
    public void ClearAllItemBuffs()
    {
        itemBuffSum = new StatusData();
        activeBuffs.Clear();
        CalculateTotalStatus();
        Debug.Log("[StatusCalc] All item buffs cleared");
    }

    /// <summary>
    /// 装備の合計ステータスを取得(UI表示用)
    /// </summary>
    public StatusData GetEquipSum()
    {
        return equipSum;
    }

    /// <summary>
    /// アイテムバフの合計ステータスを取得(デバッグ用)
    /// </summary>
    public StatusData GetItemBuffSum()
    {
        return itemBuffSum;
    }

    /// <summary>
    /// アクティブなバフの数を取得(デバッグ用)
    /// </summary>
    public int GetActiveBuffCount()
    {
        return activeBuffs.Count;
    }
}
