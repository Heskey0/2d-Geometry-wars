using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class left : MonoBehaviour
{
    PlayerController pc;
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }
    void Update()
    {
        
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "floor")
        {
            if (Input.GetKey(KeyCode.A))
                pc.is_againstingWall = true;
            else pc.is_againstingWall = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "floor")
        {
            pc.is_againstingWall = false;
        }
    }
}
