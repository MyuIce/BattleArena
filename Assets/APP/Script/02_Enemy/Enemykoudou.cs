using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemykoudou : MonoBehaviour,IEnemy
{
    
    [SerializeField] Charadata charadata;
    [SerializeField] GameObject Player;
    Vector3 distanceToPlayer; 
    int State;
    
    public int EnemyAIaction()
    {
        var Status = charadata.GetCharaStatus();
        float range = charadata.GetRawStatus().ShortAttackRange;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);

        if(distanceToPlayer <= range)
        {
            State = 1;
        }
        else
        {
            State = 0;
        }
        return State;

    }
}