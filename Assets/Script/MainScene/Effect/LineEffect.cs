using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffect : MonoBehaviour
{
    [Header("���")]Transform origin_trans;
    [Header("�յ�")][HideInInspector]public Transform target_trans;
    [Header("��������")]public float lifetime;
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
