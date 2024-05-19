using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckl : MonoBehaviour
{
    MovementNoAim playerMovement;
    void Start()
    {
        playerMovement = GetComponentInParent<MovementNoAim>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
        {
            return;
        }
        playerMovement.setGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
        {
            return;
        }

        playerMovement.setGroundedState(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject)
        {
            return;
        }
        playerMovement.setGroundedState(true);
    }
}
