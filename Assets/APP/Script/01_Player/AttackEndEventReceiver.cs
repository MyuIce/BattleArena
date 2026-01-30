// HitEventReceiver.cs
using UnityEngine;

public class AttackEndEventReceiver : MonoBehaviour
{
    public PlayerMovement movement;

    public void AttackEnd()
    {
        if (movement != null)
            movement.OnAttackEndFromAnimation();
        
    }
}
