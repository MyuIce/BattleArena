using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの最終ステータスを保持・取得するクラス
/// 計算はStatusCalcで行い、このクラスは結果を保持するのみ
/// </summary>
public class TotalRuntimeStatus : MonoBehaviour
{
    // 最終的な合計ステータス(キャラ + 装備 + アイテムバフ)
    public StatusData TotalStatus { get; private set; }

    private void Awake()
    {
        // 初期化
        TotalStatus = new StatusData();
    }

    /// <summary>
    /// 最終ステータスを設定
    /// StatusCalcから呼び出される
    /// </summary>
    public void SetTotalStatus(StatusData newStatus)
    {
        TotalStatus = newStatus;

        Debug.Log($"[RuntimeStatus] Total Status Updated: " +
                  $"ATK={TotalStatus.ATK}, DEF={TotalStatus.DEF}, " +
                  $"AGI={TotalStatus.AGI}, INT={TotalStatus.INT}, RES={TotalStatus.RES}");
    }

    /// <summary>
    /// ステータスが初期化されているか確認
    /// </summary>
    public bool IsInitialized()
    {
        return TotalStatus.ATK > 0 || TotalStatus.DEF > 0 || 
               TotalStatus.AGI > 0 || TotalStatus.INT > 0 || TotalStatus.RES > 0;
    }

    /// <summary>
    /// 安全にステータスを取得(初期化されていない場合は警告)
    /// </summary>
    public StatusData GetTotalStatus()
    {
        if (!IsInitialized())
        {
            Debug.LogWarning("[RuntimeStatus] TotalStatusがまだ初期化されていません!");
        }
        return TotalStatus;
    }
}
