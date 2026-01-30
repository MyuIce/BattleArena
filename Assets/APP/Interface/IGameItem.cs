using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===============================
//ゲームアイテムインタフェース
//===============================
public interface IGameItem
{
    string GetItemname();
    Sprite GetItemicon();
    string GetItemexplanation();
}
