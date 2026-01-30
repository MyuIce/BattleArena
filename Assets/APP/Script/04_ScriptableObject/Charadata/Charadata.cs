using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================
//�L�����f�[�^��ScriptableObject
//============================================

[CreateAssetMenu(menuName= "Data/Create StatusData")]

public class Charadata : ScriptableObject 
{
    // �� �L�����̃X�e�[�^�X���́i�C���X�y�N�^�Őݒ�\�j
    [SerializeField] private CharaStatus charaStatus;

    [System.Serializable]
    public class CharaStatus
    {
        public string NAME; //�L������
        public int MAXHP; //�ő�HP
                          //public int MAXMP; //�ő�MP
        public int ATK; //�U����
        public int DEF; //�h���
        public int INT; //����
        public int RES; // ���@��R��
        public int AGI; //�ړ����x
        public int LV; //���x��
                       //public int GETEXP; //�擾�o���l
                       //public int GETGOLD; //�擾�S�[���h
        public float ShortAttackRange; //�F������
        public float Enemytime; //��������
                                //public int EXP;//���l���o���l
    }
    

    // �� �X�e�[�^�X��Ԃ��֐�
    /*public CharaStatus GetCharaStatus()
    {
        return charaStatus; //�ϐ�����Ԃ��i�^���ł͂Ȃ��j
    }
    */
    
    //装備やステータス変更に必要な情報の構造体
    public StatusData GetCharaStatus()
    {
        return new StatusData
        {
            ATK = charaStatus.ATK,
            DEF = charaStatus.DEF,
            INT = charaStatus.INT,
            RES = charaStatus.RES,
            AGI = charaStatus.AGI
        };
    }
    public CharaStatus GetRawStatus()
    {
        return charaStatus;
    }
    
    
}
