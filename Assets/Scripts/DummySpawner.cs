using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private int spawnCount = 20;
    [SerializeField] private float distance = 4f;

    private List<GameObject> _dummies = new List<GameObject>();
    private Vector3[] _spawnPositions;
    private Camera _mainCamera;

    #endregion

    #region UnityMethods

    void Start()
    {
        GetMainCam();
        Spawn();
    }

    #endregion

    #region HelpMethods

    private void GetMainCam()
    {
        _mainCamera = Camera.main;
    }

    private void Spawn()
    {
        _spawnPositions = new Vector3[spawnCount * spawnCount];
        //Debug.LogFormat("[Choi Hong : DummySpawner - Spawn ] spawnPointZ : {0} spawnCount : {1}", spawnPointZ, spawnCount);
        var camPos = _mainCamera.transform.position;
        var basePoint = camPos - new Vector3(spawnCount * 0.5f, 0, spawnCount * 0.5f);

        for (int i = 0; i < spawnCount; i++)
        {
            for (int j = 0; j < spawnCount; j++)
            {
                if(i == spawnCount / 2 && j == spawnCount / 2) continue;
                var spawnPoint = basePoint + new Vector3(i * distance, 0, j * distance);
                var go = Instantiate(dummyPrefab, spawnPoint, Quaternion.identity, transform);

                _dummies.Add(go);
                _spawnPositions[i] = spawnPoint;
            }
        }
    }

    public void ResetPosition()
    {
        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            _dummies[i].transform.position = _spawnPositions[i];
        }
    }

    #endregion
}