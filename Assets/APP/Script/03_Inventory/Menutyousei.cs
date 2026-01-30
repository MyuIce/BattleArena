using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//====================================
//EscでのMenuの開閉、マウスカーソル表示(戦闘中用)
//====================================

public class Menutyousei : MonoBehaviour
{
    [SerializeField] GameObject MenuObject;
    [Header("各メニューのtoggle")]
    [SerializeField] private Toggle charaToggle;
    [SerializeField] private Toggle soubiToggle;
    [SerializeField] private Toggle itemToggle;

    [Header("各メニューの中身")]
    [SerializeField] private GameObject charaInside;
    [SerializeField] private GameObject soubiInside;
    [SerializeField] private GameObject itemInside;
    bool menuzyoutai;

    void Update()
    {
        //ESCでのメニューの開閉
        if (menuzyoutai == false)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                MenuObject.gameObject.SetActive(true);
                menuzyoutai = true;

                if(Camera.main == null) return;
                // マウスカーソルを表示にし、位置固定解除
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            if (Input.GetButtonDown("Cancel"))
            {
                MenuObject.gameObject.SetActive(false);
                menuzyoutai = false;

                if(Camera.main == null) return;
                // マウスカーソルを非表示にし、位置を固定
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    //アイテム・装備・キャラtoggleのメニュー開閉
    public void toggle_menu()
    {

        // キャラメニューON時
        if (charaToggle.isOn)
        {
            charaInside.SetActive(true);
            soubiInside.SetActive(false);
            itemInside.SetActive(false);
        }
        // 装備メニューON時
        else if (soubiToggle.isOn)
        {
            charaInside.SetActive(false);
            soubiInside.SetActive(true);
            itemInside.SetActive(false);
        }
        // アイテムメニューON時
        else if (itemToggle.isOn)
        {
            charaInside.SetActive(false);
            soubiInside.SetActive(false);
            itemInside.SetActive(true);
        }
    }
    //メニュー状態を返す
    public bool menuhiraki()
    {
        return menuzyoutai;
    }

}