using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange : MonoBehaviour
{
    public GameObject ResultData;

    private static SceneChange sc;
    public static SceneChange SC
    {
        get { return sc; }
    }

    private void Awake()
    {
        sc = GetComponent<SceneChange>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Click_Original()
    {
        SceneManager.LoadScene("Original");
    }

    public void Click_Dynavision()
    {
        SceneManager.LoadScene("Dynavision");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Original");
        Time.timeScale = 1f;
    }

    public void RestartDy()
    {
        SceneManager.LoadScene("Dynavision");
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetType(int num)
    {
        SceneManager.LoadScene("Dynavision");
        GameManager.type = num;
    }

    public void RightPage()
    {
        Debug.Log("눌림");
        ResultData.transform.localPosition = new Vector3(-1920f, 0f, 0f);
    }

    public void LeftPage()
    {
        ResultData.transform.localPosition = new Vector3(0f, 0f, 0f);
    }


}
