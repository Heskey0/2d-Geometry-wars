using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class right : MonoBehaviour
{
    PlayerController pc;
    Rigidbody2D rb;
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "floor")
        {
            if (Input.GetKey(KeyCode.D))
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
