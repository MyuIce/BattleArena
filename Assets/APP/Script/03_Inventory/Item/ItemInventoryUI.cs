using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


/// <summary>
/// アイテムインベントリUI
/// 
/// slotParent
/// slotIcons
/// toggleGroup
/// itemNameText
/// itemDescriptionText
/// UpdateUI()
/// UpdateSelectedSlot()
/// 
/// </summary>
public class ItemInventoryUI : MonoBehaviour
{
    [Header("スロットの親オブジェクト(25個分のImage)")]
    [SerializeField] private Transform slotParent;

    [Header("トグルグループとアイテム説明UI")]
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    private List<Image> slotIcons = new();
    private ItemInventory inventory;

    public void Initialize(ItemInventory inv)
    {
        inventory = inv;
        
        // データ変更時にUIを更新するように登録
        inventory.OnInventoryChanged -= UpdateUI; // 二重登録防止
        inventory.OnInventoryChanged += UpdateUI;

        slotIcons = slotParent.GetComponentsInChildren<Image>(true)
                              .Where(img => img.name.StartsWith("icon"))
                              .ToList();

        //トグルの値を変更した時にUpdateSelectedSlotを呼び出す
        foreach (var toggle in slotParent.GetComponentsInChildren<Toggle>())
            toggle.onValueChanged.AddListener(delegate { UpdateSelectedSlot(); });

        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
        UpdateSelectedSlot();
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時にイベント購読を解除（メモリリーク・エラー防止）
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateUI;
        }
    }

    public void UpdateUI()
    {
        if (inventory == null) return;
        var ownedItems = inventory.GetOwnedItems();
        for (int i = 0; i < slotIcons.Count; i++)
        {
            // Unityオブジェクト（Image）が破棄されていないかチェック
            if (slotIcons[i] == null) continue;

            if (i < ownedItems.Count)
            {
                slotIcons[i].sprite = ownedItems[i].GetItemicon();
                slotIcons[i].color = Color.white;              
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = new Color(0.22f, 0.22f, 0.22f, 1f);
            }
        }
    }

    public void UpdateSelectedSlot()
    {
        Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (selectedToggle == null || inventory == null) return;

        if (!int.TryParse(selectedToggle.name, out int slotNumber)) return;
        int index = slotNumber - 1;

        var owned = inventory.GetOwnedItems();
        if (index < 0 || index >= owned.Count)
        {
            // アイテムがないスロットの場合はテキストをクリア
            itemNameText.text = "";
            itemDescriptionText.text = "";
            return;
        }

        Itemdata1 item = owned[index];
        itemNameText.text = $"{item.GetItemname()} ~ {inventory.GetitemCount(item)}";
        itemDescriptionText.text = item.GetItemexplanation();
    }
}
