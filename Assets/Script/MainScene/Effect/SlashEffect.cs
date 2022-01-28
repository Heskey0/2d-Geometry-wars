using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        transform.localPosition -= transform.right * Time.deltaTime * 10;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="emy_bullet")
        {
            GameObject.Destroy(other.gameObject);
        }
        EnemyController emy_c = other.GetComponent<EnemyController>();
        if (emy_c != null)
        {
            Destroy(gameObject);
        }
    }
}
