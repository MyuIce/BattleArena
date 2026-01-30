using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CharaAttack : MonoBehaviour
{
    [SerializeField] private Charadata charaData;
    [SerializeField] GameObject AttackChara;

    int HHcount;
    int ATK;
    ICharaAttack Hcount;

    void Start()
    {
        var CharaStatus = charaData.GetCharaStatus();
       
        //敵の攻撃力
        ATK = CharaStatus.ATK;
        Hcount = AttackChara.GetComponent<ICharaAttack>();
    }
    
    void OnTriggerEnter(Collider other)
    {

        HHcount = Hcount.HitCount();
        if (HHcount <= 0)
        {

            return;
        }
        if (other.tag == "Player")
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Hcount.HitCountdown();
                damageable.Damage(ATK);
            }
        }
    }
}