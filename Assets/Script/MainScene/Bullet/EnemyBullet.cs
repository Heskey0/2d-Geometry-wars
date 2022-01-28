using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    GameObject playerObj;
    PlayerController pc;
    Vector3 player_pos;
    Vector3 dir;
    float angle;
    public float speed;
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        pc = playerObj.GetComponent<PlayerController>();
        player_pos = playerObj.GetComponent<Transform>().position;
        dir = (player_pos - transform.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0, 0, angle-90);
    }
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!pc.is_invincible)
                pc.current_health -= 10;
        }
        if (other.tag=="Player"||other.tag=="floor")
        {
            /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
            GameObject.Destroy(gameObject);
        }
    }


    //IEnumerator Wait()
    //{
    //    Time.timeScale = 0.1f;
    //    yield return new WaitForSecondsRealtime(0.1f);
    //    Time.timeScale = 1;
    //}
}
