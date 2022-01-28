using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foot : MonoBehaviour
{
    PlayerController pc;
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "floor")
        {
            pc.is_onFloor = true;
            pc.jumpTimer = pc.jumpTimes;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "floor")
        {
            pc.jumpTimer -= 1;
            pc.is_onFloor = false;
            
        }
    }
}
