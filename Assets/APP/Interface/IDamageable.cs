using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===============================
// ダメージを受けるインターフェース
// 攻撃を受けられるオブジェクトが実装する
//===============================
public interface IDamageable
{
    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="value">受けるダメージ量</param>
    void Damage(int value);
}
