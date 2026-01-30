using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeDirection : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        canvas.transform.rotation = mainCamera.transform.rotation;   
    }
    void LateUpdate()
    {
        if (mainCamera != null && canvas != null)
        {
            // EnemyGaugeをMainCameraへ向かわせる
            canvas.transform.rotation = mainCamera.transform.rotation;
        }
    }
}
