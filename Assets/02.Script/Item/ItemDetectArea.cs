using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetectArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            ItemObject itemObj = other.GetComponent<ItemObject>();

            itemObj.playerTr = this.transform.parent.transform;
            itemObj.StartCoroutine("ChasePlayer");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            ItemObject itemObj = other.GetComponent<ItemObject>();
            itemObj.StopCoroutine("ChasePlayer");
        }
    }
}
