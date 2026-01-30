// HitEventReceiver.cs
using UnityEngine;

public class HitEventReceiver : MonoBehaviour
{
    public PlayerMovement movement;

    public void Hit()
    {
        if (movement != null)
            movement.OnHitFromAnimation();
    }
}
