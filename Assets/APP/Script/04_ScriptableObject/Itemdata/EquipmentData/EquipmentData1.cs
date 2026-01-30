using UnityEngine;


[CreateAssetMenu(menuName = "Data/Create EquipmentData")]
public class EquipmentData1 : ScriptableObject, IGameItem
{
    public enum Equipmenttype
    {
        Sword, Head, Body, Leg, Crent
    }

    //============================================
    //装備情報定義
    //============================================

    [SerializeField]
    private string Equipmentname; //名前
    [SerializeField]
    private Equipmenttype equipmenttype; //タイプ
    [SerializeField]
    private Sprite Equipmenticon; //アイコン
    [SerializeField]
    private string Equipmentexplanation; //説明
    [SerializeField]
    private int Itemlimit; //最大個数
    [SerializeField]
    private Status equipmentStatus;//装備ステータス

    [System.Serializable]
    public class Status
    {
        public int ATK;
        public int DEF;
        public int AGI;
        public int INT;
        public int RES;
    }

    public string GetItemname()
    {
        return Equipmentname;
    }
    public Equipmenttype GetItemtype()
    {
        return equipmenttype;
    }
    public Sprite GetItemicon()
    {
        return Equipmenticon;
    }
    public string GetItemexplanation()
    {
        return Equipmentexplanation;
    }
    public int GetItemlimit()
    {
        return Itemlimit;
    }
    public Status GetItemStatus()
    {
        return equipmentStatus;
    }
}