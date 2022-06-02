using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FromCameraDistance : MonoBehaviour
{
    public Transform _zombie;
    private TextMeshProUGUI _text; 
    
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _zombie = GameObject.Find("ZombieContainer").transform;
    }

    
    void Update()
    {
        var camPos = Camera.main.transform.position;
        var zombiePos = _zombie.position;
        var distance = Vector3.Distance(camPos, zombiePos);
        _text.text =
            "MainCam Pos : " + camPos.ToString()+ System.Environment.NewLine+
            "Zombie Pos  : " + zombiePos.ToString()+ System.Environment.NewLine+
            "Distance    : " + distance.ToString();
    }
}
