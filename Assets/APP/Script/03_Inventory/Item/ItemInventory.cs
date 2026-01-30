using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// アイテムのデータ管理クラス。これとUIをItemManagerで管理する。
/// ItemCounts
/// ownrdItems
/// InitializeInventory()
/// AddItem()
/// GetItemCount()
/// 
/// </summary>
[System.Serializable]
public class ItemInventory
{
    
    // 所持数
    private readonly Dictionary<Itemdata1, int> itemCounts = new();
    private readonly List<Itemdata1> ownedItems = new();
    
    public System.Action OnInventoryChanged;

    public void Initialize(List<Itemdata1> itemList)//InitializeInventory()
    {
        itemCounts.Clear();
        foreach (var item in itemList)
        {
            itemCounts[item] = 0;
        }
        OnInventoryChanged?.Invoke();
    }

    public bool AddItem(Itemdata1 item,int quantity)
    {
        if (item == null) return false;
        if (!itemCounts.ContainsKey(item)) return false;

        itemCounts[item] += quantity;
        OnInventoryChanged?.Invoke();
        return true;

    }

    public List<Itemdata1> GetOwnedItems()
    {
        List<Itemdata1> owned = new();
        foreach (var kvp in itemCounts)
        {
            if (kvp.Value > 0)
                owned.Add(kvp.Key);
        }
        return owned;
    }

    public int GetitemCount(Itemdata1 item)
    {
        if (item == null) return 0;
        return itemCounts.TryGetValue(item,out int count) ? count : 0;
    }

}
