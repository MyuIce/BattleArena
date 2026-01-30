using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryTimeEventReceiver : MonoBehaviour
{
    public PlayerMovement movement;

    public void ParryTime()
    {
        if (movement != null)
            movement.ParryTime();
    }

}
