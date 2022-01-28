using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet2 : MonoBehaviour
{
    GameObject playerObj;
    PlayerController pc;
    Vector3 dir;
    public float speed;
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        pc = playerObj.GetComponent<PlayerController>();

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
        if (other.tag == "Player" || other.tag == "floor")
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
