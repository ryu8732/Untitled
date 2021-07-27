using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private Transform cameraTr;

    private void Start()
    {
        cameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(cameraTr.position.x, transform.position.y, cameraTr.position.z);
        this.transform.LookAt(targetPos);
        transform.Rotate(0f, 180f, 0f);
    }
}
