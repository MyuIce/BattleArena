using UnityEngine;

public class GuardEndEventReceiver : MonoBehaviour
{
    public PlayerMovement movement;

    public void OnGuardEnd()
    {
        if (movement != null)
        {
            movement.OnGuardEnd();
        }
    }
}