using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [Header("敌人血量")]public Slider emy_slider;
    [Header("玩家血量")]public PlayerController pc;
    public GameObject win;
    public GameObject not_win;
    void Start()
    {
        
    }


    void Update()
    {
        if (emy_slider.value<=0.001f)
        {
            StartCoroutine(waitForLoadScene());
        }
        if (pc.current_health<=0.1f)
        {
            StartCoroutine(waitForLoadScene2());
        }
    }

    IEnumerator waitForLoadScene()
    {
        win.gameObject.SetActive(true);
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        win.gameObject.SetActive(false);
        SceneManager.LoadScene("Main");
        
        yield return null;
    }
    IEnumerator waitForLoadScene2()
    {
        not_win.gameObject.SetActive(true);
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        not_win.gameObject.SetActive(false);
        SceneManager.LoadScene("Main");

        yield return null;
    }
}
