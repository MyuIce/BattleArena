using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class SlotUI
{
    public TextMeshProUGUI nameText; // 「未装着」→ 装備名
    public Image iconImage;
}

public class Soubikanri : MonoBehaviour, IInventoryManager<EquipmentData1>
{
    [Header("データベース参照")]
    [SerializeField] private EquipmentDatabase equipmentDatabase;

    [Header("UI参照")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject elementPrefab;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("アイテムステータス表示UI")]
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI agiText;
    [SerializeField] private TextMeshProUGUI intText;
    [SerializeField] private TextMeshProUGUI resText;
    [SerializeField] private Image EquipmentImage;

    // 「装備の所持数」
    private Dictionary<EquipmentData1, int> equipmentCounts;
    //「現在装備中の装備」
    private Dictionary<EquipmentData1.Equipmenttype, EquipmentData1> equipped;

    //装備の種類(EquipmentData1[ScriptableObject]のEquipmenttypeをcurrentTypeとして保存)
    private EquipmentData1.Equipmenttype currentType;

    [SerializeField] private GameObject playerManagerObject;
    
    //選択中の行（右リストで選んだ装備）
    private EquipmentData1 selected;

    //中央の5スロット（enumの順 0:武器,1:頭,2:体,3:脚,4:紋 で割り当て）
    [SerializeField] private SlotUI[] slotUIs;
    //ステータス計算クラス
    [SerializeField] private StatusCalc statusCalc;

    [Header("空スロットの設定")]
    [SerializeField] private Sprite emptySlotSprite; // 未装着時に表示する画像 (Bars_Unity_20等)

    //==============================================================
    // 初期化
    //==============================================================
    
    private void Awake()
    {
        //シングルトン(InventoryManager)を取得
        if (InventoryManager.Instance != null)
        {
            equipmentCounts = InventoryManager.Instance.EquipmentCounts;
            equipped = InventoryManager.Instance.EquippedItems;
            //UpdateEquippedSlotsUI();
            Debug.Log("[Soubikanri] InventoryManagerのデータに接続しました");
        }
        else
        {
            Debug.LogError("[Soubikanri] InventoryManagerが見つかりません！フォールバック用のローカルデータを使用します。");
            // フォールバック: ローカルデータを使用（InventoryManagerがない場合のみ）
            equipmentCounts = new Dictionary<EquipmentData1, int>();
            equipped = new Dictionary<EquipmentData1.Equipmenttype, EquipmentData1>();
        }
    }

    public void Start()
    {
        RestoreEquippedUI();
        // Initialize(); 
    }
    /// <summary>
    /// 呼び出した時に装備の所持数をすべて0にする関数(シングルトンで保存するため現在は使用していない)
    /// </summary>
    public void Initialize()
    {
        if (equipmentCounts == null) return;
        
        equipmentCounts.Clear();
        foreach (var eq in equipmentDatabase.GetItemLists())
            equipmentCounts.Add(eq, 0);
    }

    /// <summary>
    /// 装備の入手した時の所持数を増やす処理(+1とUI更新)
    /// </summary>
    public bool AddItem(EquipmentData1 item, int amount)
    {
        if (!equipmentCounts.ContainsKey(item))
            equipmentCounts[item] = 0; 
            //return false;
        equipmentCounts[item] += 1;
        UpdateUI();
        return true;
    }

    /// <summary>
    ///今現在の装備の所持数を返す処理(装備可能判定やUI表示)
    /// </summary>
    public int GetItemCount(EquipmentData1 item)
    {
        //TryGetValueはC＃のDictionary<TKey, TValue>に用意されている関数
        return equipmentCounts.TryGetValue(item, out int count) ? count : 0;

    }

    //呼び出した時に数値をEquipmentData1のenumにキャスト(0←武器、1←頭、2←体　等)
    public void OnCategoryButtonPressed(int index)//UnityでButtonを押した時のOnClick処理の番号を整数として渡すことができる(int index)
    {
        currentType = (EquipmentData1.Equipmenttype)index;
        selected = null;
        UpdateUI();
    }


    //============================================
    //UI更新
    //============================================
    public void UpdateUI()
    {
        //デバッグ用ログ
        Debug.Log($"[DEBUG] UpdateUI 呼び出し: currentType={currentType}");
        if (equipmentDatabase == null) Debug.LogError("equipmentDatabase が未設定です！");
        if (equipmentCounts == null) Debug.LogError("equipmentCounts が null です！");
        if (contentParent == null) Debug.LogError("contentParent が未設定です！");
        if (elementPrefab == null) Debug.LogError("elementPrefab が未設定です！");

        //contentParent←ScrollViewのContent　に並ぶ装備リストを全削除後に再生成
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);


        //画面に表示するべき装備を抽出
        //equipmentCounts(データベース)を走査→kv.Valueが1以上かつcurrentTypeに一致する装備のみ抽出し、リスト化(リスト化なのでUI更新ではない)
        var filtered = equipmentCounts
            .Where(kv => kv.Value > 0 && kv.Key.GetItemtype() == currentType)
            .Select(kv => kv.Key)
            .ToList();

        

        //プレイヤーの所持している装備を表示し、そのボタンを押したときにその装備の説明・ステータス・アイコンを表示する
        //filterd←現在表示するべき装備リスト(直前で指定している)
        foreach (var eq in filtered)
        {
            //プレハブの生成(Instantiate←オブジェクトを動的に生成する関数)
            //第1引数を第2引数に生成(装備ブレハブをスクロールビューに生成)
            var obj = Instantiate(elementPrefab, contentParent);

            //生成したオブジェクトの階層から変数を作成
            var nameObj = obj.transform.Find("Itemname");//装備名
            var iconObj = obj.transform.Find("EquipmentIcon");//アイコン
            var button = obj.GetComponent<Button>();

            var equipBtnTr = obj.transform.Find("Soubityuu/E");
            Button equipBtn = equipBtnTr ? equipBtnTr.GetComponent<Button>() : null;
            GameObject equipBtnGO = equipBtn ? equipBtn.gameObject : null;

            if (nameObj == null) Debug.LogError(" 'Itemname' が Prefab 内で見つかりません。");
            if (iconObj == null) Debug.LogError(" 'EquipmentIcon' が Prefab 内で見つかりません。");
            if (button == null) Debug.LogError(" Button コンポーネントが見つかりません。");

            //E
            if (equipBtnGO) equipBtnGO.SetActive(false);

            // 名前の取得(装備のScriptableObjectから)と表示
            if (nameObj != null)
            {
                var nameText = nameObj.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                    nameText.text = eq.GetItemname();
            }

            // アイコンを設定(装備のScriptableObjectから)と表示
            if (iconObj != null)
            {
                var image = iconObj.GetComponent<Image>();
                if (image != null)
                    image.sprite = eq.GetItemicon();
            }
            //装備選択をしたときのEボタンの表示
            if (button)
            {
                button.onClick.AddListener(() =>
                {
                    selected = eq;
                    ShowDetailOnTop(eq);
                    RefreshEButtons(); // E表示の付け替え
                });
            }
            //装備ボタンのEボタンを押したときにキャラに装備を装着/解除
            if (equipBtn)
            {
                equipBtn.onClick.AddListener(() =>
                {
                    if (selected == eq)
                    {
                        // 既に装備中なら外す、そうでなければ装備する
                        if (equipped.ContainsKey(currentType) && equipped[currentType] == eq)
                        {
                            Unequipment();
                        }
                        else
                        {
                            EquipCurrentType(eq);
                            SoundManager.Instance.OnEquipmentSetSE_Global();
                        }
                        RefreshEButtons();
                    }
                    
                });
            }
            if (equipBtnGO) equipBtnGO.SetActive(selected == eq);

            // ボタンが押されたときの動作を登録
            if (button != null)
            {
                //()=>は関数を登録しておき動作をしたら(今回はボタンを押したら)実行する
                //AddListener()はUnityで用意されている関数
                button.onClick.AddListener(() =>
                {
                    Debug.Log($"[DEBUG] {eq.GetItemname()} を選択しました");
                    descriptionText.text = eq.GetItemexplanation();
                });
            } 
        }
       
    }


    // -----------------------------------------------------------------------------------------
    // 上部UI（説明文・ステータス・アイコン）を更新する関数(ステータスの計算ではなくUIの表示のみ)
    // -----------------------------------------------------------------------------------------
    private void ShowDetailOnTop(EquipmentData1 eq)
    {
        // 装備のスクリプト(ScriptableObject)からGetItemexplanation()を取得し、descriptionTextに表示
        descriptionText.text = eq.GetItemexplanation();

        // ステータス表示
        var status = eq.GetItemStatus();//GetItemStatus()からキャラのステータス(int型)をstatusで取得
        atkText.text = $"+{status.ATK}";
        defText.text = $"+{status.DEF}";
        agiText.text = $"+{status.AGI}";
        intText.text = $"+{status.INT}";
        resText.text = $"+{status.RES}";

        // アイコン更新（上の装備プレビュー）
        if (EquipmentImage != null)
            EquipmentImage.sprite = eq.GetItemicon();
    }

    public void Unequipment()
    {
        if (equipped.ContainsKey(currentType))
        {
            Debug.Log($"[UNEQUIP] {currentType} の装備を外しました。");
            equipped.Remove(currentType);
            ApplyEmptyToMiddleSlot(currentType);
           
            if (statusCalc != null)
            {
                statusCalc.CalculateTotalStatus();
            }
        }
    }

    private void RefreshEButtons()
    {
        foreach (Transform row in contentParent)
        {
            var nameObj = row.Find("Itemname");
            string rowName = nameObj ? nameObj.GetComponent<TextMeshProUGUI>().text : null;
            var e = row.Find("Soubityuu/E");
            if (!e) continue;

            bool isSelectedRow = (selected != null && selected.GetItemname() == rowName);
            e.gameObject.SetActive(isSelectedRow);

            // 装備中ならボタンのテキストを「外す」に変更する
            if (isSelectedRow && selected != null)
            {
                var btnText = e.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    bool isEquipped = equipped.ContainsKey(currentType) && equipped[currentType] == selected;
                    btnText.text = isEquipped ? "脱" : "E";
                }
                
            }
            
        }
    }

    // 現在のカテゴリ（部位）に装備
    public void EquipCurrentType(EquipmentData1 eq)
    {
        equipped[currentType] = eq;
        ApplyToMiddleSlot(currentType, eq);
        

        //再計算を呼び出す（GetではなくCalculateを呼ぶ）
        if (statusCalc != null)
        {
            statusCalc.CalculateTotalStatus();
            Debug.Log("計算関数を呼び出しました。");
                    
        }
        if(statusCalc == null)
            Debug.Log("statusCalcがnullです");

        Debug.Log($"[DEBUG] 装備登録: {currentType} に {eq.GetItemname()} を登録しました。");
        Debug.Log($"[DEBUG] 現在の装備数: {equipped.Count}");
        Debug.Log($"[EQUIP] {currentType} に {eq.GetItemname()} を装備しました。");
    }

    // 中央スロットへの反映（Eボタン押下時）
    private void ApplyToMiddleSlot(EquipmentData1.Equipmenttype type, EquipmentData1 eq)
    {
        Debug.Log("ApplyToMiddleSlotを呼び出した");
        int idx = (int)type;
        if (slotUIs == null || idx < 0 || idx >= slotUIs.Length) return;

        var ui = slotUIs[idx];
        if (ui.nameText) ui.nameText.text = eq.GetItemname();
        
        if (ui.iconImage)
        {
            ui.iconImage.sprite = eq.GetItemicon();
            ui.iconImage.enabled = true; // 表示する
            var c = ui.iconImage.color; c.a = 1f; ui.iconImage.color = c;
        }
        
    }

    // 中央スロットの空欄反映（外すボタン押下時）
    private void ApplyEmptyToMiddleSlot(EquipmentData1.Equipmenttype type)
    {
        int idx = (int)type;
        if (slotUIs == null || idx < 0 || idx >= slotUIs.Length) return;

        var ui = slotUIs[idx];
        if (ui.nameText) ui.nameText.text = "未装着";
        
        if (ui.iconImage)
        {
            // 指定された初期アイコン画像（Bars_Unity_20等）に差し替え
            ui.iconImage.sprite = emptySlotSprite;
            ui.iconImage.enabled = (emptySlotSprite != null); // 画像があれば表示、なければ非表示
            
            /*
            // アルファ値を念のため戻す
            var color = ui.iconImage.color;
            color.a = 1f;
            ui.iconImage.color = color;
            */
        }
    }

    //status管理
    public Dictionary<EquipmentData1.Equipmenttype, EquipmentData1> GetEquippedItems()
    {
        return equipped; // 読み取り専用に返す
    }

    /// <summary>
    /// 中央スロットのUIを更新する
    /// </summary>
    private void RestoreEquippedUI()
    {
        if (equipped == null || equipped.Count == 0)
        {
            Debug.Log("[Soubikanri] 装備データがありません。UIは初期状態です。");
            return;
        }

        // 装備中のアイテムをすべて中央スロットに反映
        foreach (var kvp in equipped)
        {
            ApplyToMiddleSlot(kvp.Key, kvp.Value);
            Debug.Log($"[Soubikanri] UI復元: {kvp.Key} に {kvp.Value.GetItemname()} を表示");
        }
    }
}