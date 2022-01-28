using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����ת�ӵ�
/// </summary>
public class PlayerBullet : MonoBehaviour
{
    [Header("�ӵ����е�����Ч")]public GameObject bullet_effect;
    Transform target;
    [Header("�ӵ��ٶ�")]public float speed = 30;
    GameObject[] emy_objs;
    float[] distances;
    int min_distance_index;
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
        transform.forward = Vector3.Slerp(transform.forward
                            ,target.position - transform.position
                            ,0.5f / Vector3.Distance(transform.position, target.position));
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position,target.position)<0.5f)
        {
            GameObject effect = Instantiate(bullet_effect, transform.position, Quaternion.identity);
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
            if (min_inarr==arr[i])
            {
                index = i;
            }
        }
        return index;
    }
}
