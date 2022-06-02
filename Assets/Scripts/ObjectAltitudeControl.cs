using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAltitudeControl : MonoBehaviour
{
    [SerializeField] private int layer = 31;
    [SerializeField] private float moveDelay = 2f;

    private Vector3 gizmoPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;

    private bool _isMove = false;


    private void Update()
    {
        GetHitPoint();
    }

    private void GetHitPoint()
    {
        var ray = new Ray(transform.position, transform.up * -1f);
        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, 1 << layer))
        {
            var curPos = transform.position;
            newPos = gizmoPos = new Vector3(curPos.x, hitInfo.point.y, curPos.z);
            if (_isMove) return;
            
            StartCoroutine(SetPos(newPos));
        }
    }

    
    private IEnumerator SetPos(Vector3 targetPos)
    {
        Debug.LogFormat("[Choi Hong] ObjectAltitudeContol : SetPos");

        _isMove = true;
        var lerpValue = 0f;
        var curTime = 0f;
        var startPos = transform.position;
        while (lerpValue <= 1f)
        {
            curTime += Time.deltaTime;
            lerpValue = curTime / moveDelay;
            transform.position = Vector3.Lerp(startPos, targetPos, lerpValue);
            yield return null;
        }
        _isMove = false;
    }

    private void OnDrawGizmos()
    {
        if (gizmoPos == Vector3.zero) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, gizmoPos);
        Gizmos.DrawSphere(gizmoPos, 0.1f);

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawCube(newPos, boundsSize);
    }
}