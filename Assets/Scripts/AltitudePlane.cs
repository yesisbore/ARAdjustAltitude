using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AltitudePlanePoints
{
    public Vector3 nearLeft;
    public Vector3 nearRight;
    public Vector3 farLeft;
    public Vector3 farRight;
}

public class AroundAltitudePlanePoints
{
    public Vector3[] nearPoints;
    public Vector3[] farPoints;
}

public class AltitudePlane : MonoBehaviour
{
    #region Variables

    [HideInInspector]
    public AroundAltitudePlanePoints _aroundPlanePoints;

    public float correctionValue = 0f;
    public GameObject outBoundPrefab;

    private Camera _mainCamera;
    private Transform _camTransform;
    private float _farDist = 20f;
    private float _altitude;

    private AltitudePlanePoints _planePoints;
    private ARPlaneManager _arPlaneManager;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;

    #endregion

    #region UnityMethods

    private void Start()
    {
        InitializeSettings();
        MakeAroundAltitudePlane();
    }

    private void Update()
    {
        SetPos();
    }

    #endregion
    
    #region HelpMethod

    private void SetPos()
    {
        var camPos = _camTransform.position;
        transform.position = new Vector3(camPos.x, 0, camPos.z);
    }
    
    private void InitializeSettings()
    {
        _mainCamera = Camera.main;
        _camTransform = _mainCamera.transform;

        _planePoints = new AltitudePlanePoints();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        // Near Points = 8 , Far Points = 8
        _aroundPlanePoints = new AroundAltitudePlanePoints();
        _aroundPlanePoints.nearPoints = new Vector3[8];
        _aroundPlanePoints.farPoints = new Vector3[8];
        _vertices = new Vector3[_aroundPlanePoints.nearPoints.Length + _aroundPlanePoints.farPoints.Length];

        _arPlaneManager = GameObject.FindObjectOfType<ARSessionOrigin>().GetComponent<ARPlaneManager>();
        _arPlaneManager.planesChanged += UpdatePlaneMeshWithARPlane;

    }

    /// <summary>
    /// 카메라의 주변 영역을 덮는 메쉬를 만듬
    /// </summary>
    private void MakeAroundAltitudePlane()
    {
        SetAroundAltitudePlanePoints();
        CreateAroundPlane();
    }

    private void SetAroundAltitudePlanePoints()
    {
        var halfFov = (_mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        var aspect = _mainCamera.aspect;

        var nearDist = _mainCamera.nearClipPlane;
        var nearHeight = Mathf.Tan(halfFov) * nearDist;
        var nearWidth = nearHeight * aspect;

        var farHeight = 1; //Mathf.Tan(halfFov) * farDist;
        var farWidth = farHeight * aspect;

        SetPoints(_aroundPlanePoints.nearPoints, nearDist, nearWidth, nearHeight);
        _altitude = _aroundPlanePoints.nearPoints[0].y;
        Debug.LogFormat("[Choi Hong] AltitudePlane : Set Near Points");
        
        SetPoints(_aroundPlanePoints.farPoints, _farDist, farWidth, farHeight);
        Debug.LogFormat("[Choi Hong] AltitudePlane : Set Far Points");
    }

    private void SetPoints(Vector3[] points, float dist, float width, float height)
    {
        var cameraTransform = _mainCamera.transform;
        var pos = cameraTransform.position;
        var forward = cameraTransform.forward;
        var right = cameraTransform.right;
        var up = cameraTransform.up;

        // Get Near Points - (Vector3 pos, Vector3 right, Vector3 up, float width,float height)
        // Front 
        var startPosFront = pos + forward * dist;
        points[0] = GetPointLeft(startPosFront, right, up, width, height);
        points[1] = GetPointRight(startPosFront, right, up, width, height);

        // Right
        var startPosRight = pos + right * dist;
        var rRight = forward * -1;
        points[2] = GetPointLeft(startPosRight, rRight, up, width, height);
        points[3] = GetPointRight(startPosRight, rRight, up, width, height);

        // Back
        var startPosBack = pos + (forward * -1) * dist;
        var bRight = right * -1;
        points[4] = GetPointLeft(startPosBack, bRight, up, width, height);
        points[5] = GetPointRight(startPosBack, bRight, up, width, height);

        // Left
        var startPosLeft = pos + (right * -1) * dist;
        var lRight = forward;
        points[6] = GetPointLeft(startPosLeft, lRight, up, width, height);
        points[7] = GetPointRight(startPosLeft, lRight, up, width, height);
    }

    private Vector3 GetPointLeft(Vector3 pos, Vector3 right, Vector3 up, float width, float height)
    {
        pos -= right * width;
        pos -= up * height;
        return pos;
    }

    private Vector3 GetPointRight(Vector3 pos, Vector3 right, Vector3 up, float width, float height)
    {
        pos += right * width;
        pos -= up * height;
        return pos;
    }

    private void UpdatePoints(Vector3[] points, float adjustedValue)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(points[i].x, adjustedValue, points[i].z);
        }

        UpdateAroundPlane();
    }

    private void CreateAroundPlane()
    {
        UpdateAroundPlane();
        CreatePlaneOutBound();
    }

    private void UpdateAroundPlane()
    {
        CreateAroundMesh();
        UpdateAroundMesh();
    }

    private void CreateAroundMesh()
    {
        Array.Copy(_aroundPlanePoints.nearPoints, _vertices, _aroundPlanePoints.nearPoints.Length);
        Array.Copy(_aroundPlanePoints.farPoints, 0, _vertices, _aroundPlanePoints.nearPoints.Length,
            _aroundPlanePoints.farPoints.Length);
        
        _triangles = new int[]
        {
            0, 8, 9,
            1, 0, 9,
            1, 9, 10,
            2, 1, 10,
            3, 2, 10,
            3, 10, 11,
            4, 3, 11,
            4, 11, 12,
            5 ,4, 12,
            5, 12, 13,
            6, 5, 13,
            6, 13, 14,
            7, 6, 14,
            7, 14, 15,
            0, 7, 15,
            0, 15, 8
        };

        //Flip
        // _triangles = new int[]
        // {
        //     9, 8, 0,
        //     9, 0, 1,
        //     10, 9, 1,
        //     10, 1, 2,
        //     10, 2, 3,
        //     11, 10, 3,
        //     11, 3, 4,
        //     12, 11, 4,
        //     12, 4, 5,
        //     13, 12, 5,
        //     13, 5, 6,
        //     14, 13, 6,
        //     14, 6, 7,
        //     15, 14, 7,
        //     15, 7, 0,
        //     8, 15, 0
        // };
    }

    private void UpdateAroundMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        GetComponent<MeshCollider>().sharedMesh = _mesh;
    }

    private void CreatePlaneOutBound()
    {
        var go = Instantiate(outBoundPrefab, Vector3.zero, quaternion.identity, this.transform);

        var pos = _mainCamera.transform.forward * _mainCamera.farClipPlane;
        go.GetComponent<AltitudePlaneOutBound>().Initialize(this.transform, pos);
    }

    private void UpdatePlaneMeshWithARPlane(ARPlanesChangedEventArgs args)
    {
        var adjustedValue = GetHeightFromARPlane(args);
        if (adjustedValue >= 0f || double.IsNaN(adjustedValue)) return;

        UpdatePoints(_aroundPlanePoints.nearPoints, adjustedValue - 0.2f);
    }

    private float GetHeightFromARPlane(ARPlanesChangedEventArgs args)
    {
        var cnt = 0f;
        var adjustedValue = 0f;
        foreach (var plane in args.updated)
        {
            var planeY = plane.transform.position.y;
            if (planeY >= 0f) continue;

            adjustedValue += planeY;
            cnt++;
        }

        if (cnt >= 3)
        {
            _arPlaneManager.planesChanged -= UpdatePlaneMeshWithARPlane;
        }

        adjustedValue /= cnt;
        Debug.LogFormat("adjustedValue : {0}", adjustedValue);
        return adjustedValue;
    }

    public void SetPlaneAltitude()
    {
        UpdatePoints(_aroundPlanePoints.nearPoints, _altitude--);
    }

    #endregion
    
    #region Gizmos

    private float nearRadius = 0.03f;
    private float farRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawSphere(_planePoints.nearLeft, nearRadius);
        // Gizmos.DrawSphere(_planePoints.nearRight, nearRadius);
        foreach (var point in _aroundPlanePoints.nearPoints)
        {
            Gizmos.DrawSphere(point, nearRadius);
        }

        Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(_planePoints.farLeft, farRadius);
        // Gizmos.DrawSphere(_planePoints.farRight, farRadius);
        foreach (var point in _aroundPlanePoints.farPoints)
        {
            Gizmos.DrawSphere(point, farRadius);
        }

    }

    #endregion
}