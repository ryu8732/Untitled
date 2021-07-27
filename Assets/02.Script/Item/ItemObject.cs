using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int itemNo;
    public int amount = 1;

    [HideInInspector]
    public Transform playerTr;
    private float chaseSpeed = 5.0f;

    public IEnumerator ChasePlayer()
    {
        while(true) {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, playerTr.position, chaseSpeed * Time.deltaTime);
        }
    }

    public IEnumerator DestroyItemObj(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolingManager.instance.InsertQueue(this.gameObject, "dropItem");
    }
    
    public void SetItem(int itemNo, int amount)
    {
        this.itemNo = itemNo;
        this.amount = amount; 
    }

    public void RandomForce()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 50), 250f, Random.Range(0, 50)));
    }

    public void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            StopCoroutine(ChasePlayer());
            col.gameObject.GetComponent<PlayerStatement>().inventory.AddItem(DataManager.instance.itemDict[itemNo], amount);
            StartCoroutine(DestroyItemObj(0f));
        }
    }
}
