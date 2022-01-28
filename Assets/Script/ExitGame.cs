using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class ExitGame : MonoBehaviour
{
    public Button exitBtn;
    public GameObject PanelObj;
    void Start()
    {
        //exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        exitBtn.onClick.AddListener(Exit);
        //PanelObj = GameObject.Find("ExitPanel");
    }
    void Update()
    {
        ShowPanel();
    }
    private void Exit()
    {
        Application.Quit(0);
    }


    void ShowPanel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("a");
            if (PanelObj.activeSelf)
            {
                PanelObj.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("b");
                PanelObj.gameObject.SetActive(true);
            }
        }
    }
    /*
    private void OnGUI()
    {
        //if ((PanelObj == null)||(exitBtn == null))
        //{
        //    Debug.Log("ÒýÓÃÎª¿Õ");
        //}
        if (GUILayout.Button("Test"))
        {
            if (PanelObj.activeSelf)
            {
                PanelObj.gameObject.SetActive(true);
            }
            else
            {
                PanelObj.gameObject.SetActive(false);
            }
        }
    }
    */
}
