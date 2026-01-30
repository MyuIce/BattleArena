using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 右下にアイテムを1つずつ表示し、右クリックで切り替え、左クリックで使用するクラス
/// アイテム情報を受けわたすItemManagerから所持アイテムの情報を取得して表示する
/// ロジッククラス→ItemInventory.cs  受け渡し→ItemManager.cs
/// </summary>
public class ItemUseManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private Image itemIconImage;           
    [SerializeField] private TextMeshProUGUI itemCountText; 

    [Header("プレイヤー参照")]
    [SerializeField] private GameObject player;             

    private ItemInventory itemInventory;                    
    private List<Itemdata1> ownedItems;                     
    private int currentItemIndex = 0;                       

    void Start()
    {
        ItemManager manager = FindObjectOfType<ItemManager>();
        if (manager != null)
        {
            StartCoroutine(InitInventory(manager));
        }
        else
        {
            Debug.LogWarning("[ItemUseManager] ItemManagerが見つかりません");
        }

        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[ItemUseManager] Playerが見つかりません");
            }
        }
    }

    private IEnumerator InitInventory(ItemManager manager)
    {
        yield return null; 
        
        itemInventory = manager.GetInventory();
        
        if (itemInventory != null)
        {
            Debug.Log("[ItemUseManager] ItemManagerに接続しました");
            UpdateItemDisplay();
        }
        else
        {
            Debug.LogError("[ItemUseManager] ItemInventoryの取得に失敗しました");
        }
    }

    void Update()
    {
        // 所持アイテムがない場合は処理をスキップ
        if (ownedItems == null || ownedItems.Count == 0)
        {
            return;
        }

        // 右クリックで次のアイテムに切り替え
        if (Input.GetMouseButtonDown(1))
        {
            NextItem();
        }

        // Eキーでアイテムを使用
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[ItemUseManager] Eキーが押されました");
            UseCurrentItem();
            SoundManager.Instance.OnItemUseSE_Global();
        }
        
    }

    /// <summary>
    /// 次のアイテムに切り替え
    /// </summary>
    private void NextItem()
    {
        if (ownedItems == null || ownedItems.Count == 0) return;

        currentItemIndex = (currentItemIndex + 1) % ownedItems.Count;
        UpdateItemDisplay();
    }

    /// <summary>
    /// 現在選択中のアイテムを使用
    /// </summary>
    private void UseCurrentItem()
    {
        //所持アイテムが無いときに空return
        if (ownedItems == null || ownedItems.Count == 0) return;
        if (player == null)
        {
            Debug.LogError("[ItemUseManager] プレイヤーが設定されていません");
            return;
        }

        Itemdata1 currentItem = ownedItems[currentItemIndex];
        int count = itemInventory.GetitemCount(currentItem);

        if (count > 0)
        {
            // アイテムを使用
            currentItem.Use(player);
            
            // 所持数を減らす
            itemInventory.AddItem(currentItem, -1);
            
            Debug.Log($"[ItemUseManager] {currentItem.GetItemname()} を使用しました");

            // 表示を更新
            UpdateItemDisplay();
        }
        else
        {
            Debug.LogWarning($"[ItemUseManager] {currentItem.GetItemname()} の所持数が0です");
        }
    }
    

    /// <summary>
    /// アイテム表示を更新
    /// </summary>
    private void UpdateItemDisplay()
    {
        if (itemInventory == null) return;

        // 所持しているアイテムのリストを取得
        ownedItems = itemInventory.GetOwnedItems();

        // 所持アイテムがない場合
        if (ownedItems == null || ownedItems.Count == 0)
        {
            // アイテムがない場合は非表示にする
            if (itemIconImage != null) itemIconImage.enabled = false;
            if (itemCountText != null) itemCountText.text = "";
            return;
        }

        // インデックスが範囲外の場合は0にリセット
        if (currentItemIndex >= ownedItems.Count)
        {
            currentItemIndex = 0;
        }

        // 現在のアイテムを取得
        Itemdata1 currentItem = ownedItems[currentItemIndex];
        int count = itemInventory.GetitemCount(currentItem);

        // UIを更新
        if (itemIconImage != null)
        {
            Sprite icon = currentItem.GetItemicon();
            if (icon != null)
            {
                itemIconImage.sprite = icon;
                itemIconImage.enabled = true; 
            }
            else
            {
                Debug.LogWarning($"[ItemUseManager] {currentItem.GetItemname()} のアイコンが設定されていません");
                itemIconImage.enabled = false;
            }
        }
        else
        {
            Debug.LogError("[ItemUseManager] itemIconImage がInspectorで設定されていません");
        }

        if (itemCountText != null)
            itemCountText.text = $"x{count}";
    }

    /// <summary>
    /// 外部から表示を更新するためのメソッド（アイテム取得時などに呼び出す）
    /// </summary>
    public void RefreshDisplay()
    {
        UpdateItemDisplay();
    }
}
