using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [Header("�л�����ʱ����")]public float change_dir_time;
    [Header("�Ƿ�ֱ�ƶ�")]public bool isVertical;

    [Header("�ӵ���Ч")]public GameObject bullet;
    [Header("�������")]public float attackTime;
    float attackTimer;

    [Header("�ӵ���Ч2")]public GameObject bullet2;
    [Header("�������2")]public float attackTime2;
    float attackTimer2;

    public EnemySlider emy_health_slider;

    [Range(1,30)]public float speed;
    float change_dir_timer;
    Rigidbody2D rb;
    void Start()
    {
        //����
        attackTimer = attackTime;
        attackTimer2 = attackTime2;

        //�ƶ�
        change_dir_timer = change_dir_time;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = isVertical ? new Vector2(0, speed) : new Vector2(speed, 0);
    }
    void Update()
    {
        #region ����ı�
        change_dir_timer -= Time.deltaTime;
        if (change_dir_timer<=0)
        {
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y * -1);
            change_dir_timer = change_dir_time;
        }
        #endregion
        #region ���˹���
        //��ͨ����
        attackTimer -= Time.deltaTime;
        if (attackTimer<=0)
        {
            GameObject emyBulletObj = Instantiate(bullet, transform.position, Quaternion.identity);


            attackTimer = attackTime;
        }
        //��ǿ����
        attackTimer2 -= Time.deltaTime;
        if (attackTimer2 <= 0)
        {
            StartCoroutine(shootDeltaTime());


            attackTimer2 = attackTime2;
        }
        #endregion
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            emy_health_slider.emy_current_health -= 10;
        if (other.tag == "player_bullet")
            emy_health_slider.emy_current_health -= 2;
        if (other.tag == "bullet2")
            emy_health_slider.emy_current_health -= 3;
        if (other.tag == "player_slash")
            emy_health_slider.emy_current_health -= 2;
    }

    /********************     Э��     *********************/
    IEnumerator shootDeltaTime()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject emyBulletObj2 = Instantiate(bullet2, transform.position, Quaternion.Euler(0,0,30*i));
            yield return new WaitForSecondsRealtime(0.2f);
            if (i==11)
            {
                yield return null;
            }
        }

    }
}
