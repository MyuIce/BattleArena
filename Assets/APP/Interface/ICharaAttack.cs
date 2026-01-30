using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================
//攻撃判定インタフェース
//============================
public interface ICharaAttack
{
    public int HitCount();
    public void HitCountdown();
    public bool Attacktimeinspection();
}
