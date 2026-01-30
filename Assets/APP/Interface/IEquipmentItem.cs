using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===========================================================
//装備専用インタフェース(ゲームアイテムインタフェースを継承)
//===========================================================
public interface IEquipmentItem : IGameItem
{
    EquipmentData1.Equipmenttype GetEquipmenttype(); // 装備カテゴリ（武器・頭・胴など）
}
