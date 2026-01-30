using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create ItemDataBase")]
public class Itemdatabase : ScriptableObject
{

    [SerializeField]
    private List<Itemdata1> itemLists = new List<Itemdata1>();

    //�@�A�C�e�����X�g��Ԃ�
    public List<Itemdata1> GetItemLists()
    {
        return itemLists;
    }

}