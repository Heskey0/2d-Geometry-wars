using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    public float destory_time;
    void Start()
    {
        Destroy(gameObject, destory_time);
    }
    void Update()
    {
        
    }
}
