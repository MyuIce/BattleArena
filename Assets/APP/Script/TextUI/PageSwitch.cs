using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageSwitch : MonoBehaviour
{
    [SerializeField] private GameObject Page1Text; 
    [SerializeField] private GameObject Page2Text;

    int currentPage = 1;
    public void NextPage()
    {
        if (currentPage == 2) return;
        currentPage++;
        UpdatePage();

    }

    public void BackPage()
    {
        if(currentPage == 1) return;
        currentPage--;
        UpdatePage();
    }

    private void UpdatePage()
    {
        Page1Text.gameObject.SetActive(currentPage == 1);
        Page2Text.gameObject.SetActive(currentPage == 2);
    }
}
