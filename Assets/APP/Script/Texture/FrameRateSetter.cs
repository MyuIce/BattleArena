using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;  // ← 60FPSに固定
    }
}
