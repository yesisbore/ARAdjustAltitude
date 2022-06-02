using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AltitudePlaneManager : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private int layer = 31;
    [SerializeField] private Material altitudePalneMaterial;
    [SerializeField] private GameObject outBoundPrefab;
    
    private static AltitudePlane altitudePalne;
    
    #endregion

    #region UnityMethods

    void Start()
    {
        CreatePlane();
    }

    #endregion

    #region HelpMethods
    

    private void CreatePlane()
    {
        if (altitudePalne != null)
        {
            return;
        }
        
        var mainCam = Camera.main;
        
        // var altitudePlaneOrigin = new GameObject("AltitudePlaneOrigin");
        // altitudePlaneOrigin.AddComponent<AltitudePlaneOrigin>();
        // altitudePlaneOrigin.transform.SetParent(transform);

        var go = new GameObject("AltitudePlane");
        go.layer = layer;
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0, -mainCam.transform.position.y, -mainCam.nearClipPlane);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>().material = altitudePalneMaterial;
        go.AddComponent<MeshCollider>();
        altitudePalne = go.AddComponent<AltitudePlane>();
        altitudePalne.outBoundPrefab = outBoundPrefab;
    }
    
    public void SetPlaneAltitude()
    {
        altitudePalne.SetPlaneAltitude();
    }
    
    #endregion
}
