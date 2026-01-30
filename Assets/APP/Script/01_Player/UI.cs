using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UI : MonoBehaviour
{
    [SerializeField] Charadata Charadata;
    [SerializeField] TextMeshProUGUI NAME;
    [SerializeField] TextMeshProUGUI LV;

    // Update is called once per frame
    void Update()
    {
        var Status = Charadata.GetRawStatus();
        NAME.text = Status.NAME;
        
    }
}
