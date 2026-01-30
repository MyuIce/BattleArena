using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create EquipmentDataBase")]
public class EquipmentDatabase : ScriptableObject
{
    //EquipmentData1�Ƃ����f�[�^�x�[�X���쐬
    [SerializeField]
    private List<EquipmentData1> itemLists = new List<EquipmentData1>();

    //�@�A�C�e�����X�g��Ԃ�
    public List<EquipmentData1> GetItemLists()
    {
        return itemLists;
    }

}