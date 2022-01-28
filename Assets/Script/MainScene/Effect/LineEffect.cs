using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffect : MonoBehaviour
{
    [Header("起点")]Transform origin_trans;
    [Header("终点")][HideInInspector]public Transform target_trans;
    [Header("声明周期")]public float lifetime;
    [HideInInspector]public int index;
    LineRenderer line;

    void Awake()
    {
        index = GameObject.FindGameObjectWithTag("bullet2").GetComponent<PlayerBullet2>().min_distance_index;
    }
    void Start()
    {
        origin_trans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        target_trans = GameObject.FindGameObjectWithTag("bullet2").GetComponent<PlayerBullet2>().target;
        line = GetComponent<LineRenderer>();
        GameObject.Destroy(gameObject, lifetime);
    }
    void Update()
    {
        line.SetPosition(0, origin_trans.position);
        line.SetPosition(1, target_trans.position);
    }
}
