using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================
//インベントリ管理の共通インタフェース
//========================================

public interface IInventoryManager<T>
{
    void Initialize();//初期化（辞書クリア＋登録）
    void UpdateUI();//UI更新
    bool AddItem(T item, int amount);//アイテム追加
    int GetItemCount(T item);//所持数取得
}
