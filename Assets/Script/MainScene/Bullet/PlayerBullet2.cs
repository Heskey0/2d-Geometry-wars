using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 非旋转子弹
/// </summary>
public class PlayerBullet2 : MonoBehaviour
{
    [Header("击中时线条特效")]public GameObject line_effect;
    [Header("击中时粒子特效")]public GameObject particle_effect;
    [HideInInspector]public Transform target;
    [Header("子弹速度")]public float speed = 30;
    GameObject[] emy_objs;
    float[] distances;
    [HideInInspector]public int min_distance_index;
    void Start()
    {
        emy_objs = GameObject.FindGameObjectsWithTag("enemy");
        distances = new float[emy_objs.Length];
        for (int i = 0; i < emy_objs.Length; i++)
        {
            distances[i] = Vector3.Distance(emy_objs[i].GetComponent<Transform>().position, transform.position);
        }
        min_distance_index = Arr_min_index(distances);
        target = emy_objs[min_distance_index].GetComponent<Transform>();
    }

    void Update()
    {
        Track();
    }

    void Track()
    {
        transform.up = Vector3.Slerp(transform.up
                            , target.position - transform.position
                            , 0.5f / Vector2.Distance(transform.position, target.position));
        transform.position += transform.up * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            GameObject effect = Instantiate(line_effect, Vector3.zero, Quaternion.identity);
            GameObject effect2 = Instantiate(particle_effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    int Arr_min_index(float[] arr)
    {
        float min_inarr = arr[0];
        int index = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            min_inarr = Mathf.Min(min_inarr, arr[i]);
            if (min_inarr == arr[i])
            {
                index = i;
            }
        }
        return index;
    }
}
