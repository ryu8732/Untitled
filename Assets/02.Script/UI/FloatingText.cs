using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    void Update()
    {
        this.transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }

    public void DestroyObj(float time)
    {
        StartCoroutine(DestroyObjCoroutine(time));
    }

    private IEnumerator DestroyObjCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolingManager.instance.InsertQueue(gameObject, "floatingText");
    }
}
