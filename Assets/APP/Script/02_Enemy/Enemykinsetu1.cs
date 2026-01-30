using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ICharaAttackのインターフェースを宣言しているので、public int HitCount();とpublic void HitCountdown();のメソッドを作る必要あり。
public class Enemykinsetu1 : MonoBehaviour,ICharaAttack
{
    Animator animator;
    int Hcount;   //攻撃ヒット回数
    bool Attacktime;   //攻撃中

    public int HitCount()
    {
        //現在の残りヒット数を返す。
        return Hcount;
    }

    public void HitCountdown()
    {
        
        //ダメージが入ることが確定した際に残りヒット数を減らす。
        if(Hcount > 0)
        {
            --Hcount;
        }
        

    }
    public bool Attacktimeinspection()
    {
        //攻撃中か攻撃中でないかを返す。
        return Attacktime;

    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AttackStart()
    {

        //剣振り開始
        Hcount = 1;
        Attacktime = true;
    }
    public void Hit()
    {
        //剣振り終了
        Hcount = 0;
        Attacktime = false;
    }
    public void AttackEnd()
    {
        //アニメーション終了 Attackパラメータを0にする。
        //animator.SetFloat("Attack",0f);
        Debug.Log($"[{gameObject.name}] AttackEnd() called - Attack parameter set to 0");
    }


}