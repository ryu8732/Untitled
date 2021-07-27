using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractArea : MonoBehaviour
{
    public Action<Collider> CollisionEnterEvent;
    public Action<Collider> CollisionExitEvent;

    private void OnTriggerEnter(Collider collider) {
        if (collider.transform.tag == "Player")
        {
            CollisionEnterEvent(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            CollisionExitEvent(collider);
        }
    }
}
