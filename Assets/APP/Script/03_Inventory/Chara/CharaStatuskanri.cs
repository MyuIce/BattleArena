using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//=========================================================
//ステータスメニューのUI表示部分
//=========================================================

public class CharaStatuskanri : MonoBehaviour
{
    [Header("合計ステータスUI")]
    [SerializeField] private TextMeshProUGUI totalATKText;
    [SerializeField] private TextMeshProUGUI totalDEFText;
    [SerializeField] private TextMeshProUGUI totalAGIText;
    [SerializeField] private TextMeshProUGUI totalINTText;
    [SerializeField] private TextMeshProUGUI totalRESText;

    [Header("キャラステータスUI")]
    [SerializeField] private TextMeshProUGUI basicATKText;
    [SerializeField] private TextMeshProUGUI basicDEFText;
    [SerializeField] private TextMeshProUGUI basicAGIText;
    [SerializeField] private TextMeshProUGUI basicINTText;
    [SerializeField] private TextMeshProUGUI basicRESText;
    
    [Header("装備ステータスUI")]
    [SerializeField] private TextMeshProUGUI equipmentATKText;
    [SerializeField] private TextMeshProUGUI equipmentDEFText;
    [SerializeField] private TextMeshProUGUI equipmentAGIText;
    [SerializeField] private TextMeshProUGUI equipmentINTText;
    [SerializeField] private TextMeshProUGUI equipmentRESText;


    [Header("キャラステータスデータ")]
    [SerializeField] private Charadata charadata;
    [SerializeField] private StatusCalc statusCalc; // ステータスの足し算

    

    void Start()
    {
        if (charadata != null)
        {
            UpdateStatusDisplay();
        }
        if(charadata == null)
        {
            Debug.Log("CharaStatuskanri��charadata��null�ł�"); 
        }
        
    }

    // UIの更新
    public void UpdateStatusDisplay()
    {
        Debug.Log("[DEBUG] UpdateStatusDisplay() を実行");
        var Status = charadata.GetCharaStatus();
        var equipSum = statusCalc.GetEquipSum();

        totalATKText.text = $"ATK:{Status.ATK + equipSum.ATK}";
        totalDEFText.text = $"DEF:{Status.DEF + equipSum.DEF}";
        totalAGIText.text = $"AGI:{Status.AGI + equipSum.AGI}";
        totalINTText.text = $"INT:{Status.INT + equipSum.INT}";
        totalRESText.text = $"RES:{Status.RES + equipSum.RES}";

        basicATKText.text = $"ATK: {Status.ATK}";
        basicDEFText.text = $"DEF: {Status.DEF}";
        basicAGIText.text = $"AGI: {Status.AGI}";
        basicINTText.text = $"INT: {Status.INT}";
        basicRESText.text = $"RES: {Status.RES}";

        equipmentATKText.text = $"ATK: {equipSum.ATK}";
        equipmentDEFText.text = $"DEF: {equipSum.DEF}";
        equipmentAGIText.text = $"AGI: {equipSum.AGI}";
        equipmentINTText.text = $"INT: {equipSum.INT}";
        equipmentRESText.text = $"RES: {equipSum.RES}";
        Debug.Log($"[DEBUG] UpdateStatusDisplay() �ɓ͂��� equipSum: ATK={equipSum.ATK}, DEF={equipSum.DEF}, AGI={equipSum.AGI}, INT={equipSum.INT}, RES={equipSum.RES}");

    }

    
}

