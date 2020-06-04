using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange : MonoBehaviour
{
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

    public void SetType(int num)
    {
        SceneManager.LoadScene("Dynavision");
        GameManager.type = num;
    }

   
}
