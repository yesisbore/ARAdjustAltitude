using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudePlaneOutBound : MonoBehaviour
{
    [HideInInspector]
    public Transform altitudePlane;
    
    public void Initialize(Transform parent,Vector3 pos)
    {
        altitudePlane = parent;
        transform.position = pos;
        transform.forward = Camera.main.transform.forward;
        // transform.LookAt(altitudePlane.transform);
        // var lookRot = new Vector3(0, transform.eulerAngles.y,0);
        // transform.eulerAngles = lookRot;
    }
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Renderer>().enabled = true;
    }
}
