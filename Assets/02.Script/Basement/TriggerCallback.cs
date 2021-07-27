using System;
using UnityEngine;

public class TriggerCallback : MonoBehaviour
{
    public Action<Collider> CollisionEnterEvent;
    public Action<Collider> CollisionStayEvent;
    public Action<Collider> CollisionExitEvent;

    private void OnTriggerEnter(Collider other) { if(CollisionEnterEvent != null) CollisionEnterEvent(other); }
    private void OnTriggerStay(Collider other) { if (CollisionStayEvent != null) CollisionStayEvent(other); }
    private void OnTriggerExit(Collider other) { if (CollisionExitEvent != null) CollisionExitEvent(other); }
}
