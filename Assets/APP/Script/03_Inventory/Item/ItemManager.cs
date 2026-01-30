using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ItemInventory.csとItemInventoryUI.csのUIを制御する
/// 
/// Start()
/// AddItem()
/// </summary>

public class ItemManager : MonoBehaviour
{
    [Header("データベース参照")]
    [SerializeField] private Itemdatabase itemDatabase;
    [SerializeField] private ItemInventoryUI inventoryUI;

    private ItemInventory inventory;

    /// <summary>
    /// 外部からインベントリデータを取得するためのメソッド
    /// </summary>
    public ItemInventory GetInventory()
    {
        return inventory;
    }

    private void Start()
    {
        // InventoryManagerから共有のインベントリデータを取得
        if (InventoryManager.Instance != null)
        {
            inventory = InventoryManager.Instance.ItemInventory;
            Debug.Log("[ItemManager] InventoryManagerのインベントリに接続しました");
        }
        else
        {
            Debug.LogWarning("[ItemManager] InventoryManagerが見つかりません。ローカルで生成します。");
            inventory = new ItemInventory();
        }

        // データベースで初期化
        inventory.Initialize(itemDatabase.GetItemLists());

        // 初期アイテム指定（テスト用）
        if (itemDatabase != null && itemDatabase.GetItemLists().Count >= 2)
        {
            // すでにアイテムがある場合は二重追加を防ぐためのチェック（必要に応じて）
            if (inventory.GetOwnedItems().Count == 0)
            {
                inventory.AddItem(itemDatabase.GetItemLists()[0], 1);
                inventory.AddItem(itemDatabase.GetItemLists()[2], 2);
                inventory.AddItem(itemDatabase.GetItemLists()[4], 1);
            }
        }
        
        if (inventoryUI != null)
            inventoryUI.Initialize(inventory);
    }

    /// <summary>
    /// アイテム追加(現在は未使用)
    /// </summary>
    public bool AddItem(Itemdata1 item, int amount)
    {
        if (inventory.AddItem(item, amount))
            inventoryUI.UpdateUI();
        Debug.Log("アイテムを追加しました");
        return true;

    }
}
