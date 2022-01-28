using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player_trans;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = player_trans.position-new Vector3(0,0,100);
    }
}
