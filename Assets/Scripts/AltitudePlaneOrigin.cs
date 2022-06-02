using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudePlaneOrigin : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        SetPos();
    }
    
    private void SetPos()
    {
        var camForward = _mainCamera.transform.forward;
        var newForward = new Vector3(camForward.x, 0, camForward.z);

        var camPos = _mainCamera.transform.position;
        var newPos = new Vector3(camPos.x, camPos.y, camPos.z + Camera.main.nearClipPlane);
        
        transform.position = newPos;
        transform.forward = newForward;
    }
}
