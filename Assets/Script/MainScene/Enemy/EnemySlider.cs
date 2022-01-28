using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySlider : MonoBehaviour
{
    [HideInInspector]public float emy_current_health;
    public float emy_health;
    void Start()
    {
        emy_current_health = emy_health;
    }
    void Update()
    {
        GetComponent<Slider>().value = emy_current_health / emy_health;
    }
}
