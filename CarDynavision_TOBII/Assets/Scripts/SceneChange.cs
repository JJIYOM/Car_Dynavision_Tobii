using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 씬 관리 스크립트
 */

public class SceneChange : MonoBehaviour
{
    public GameObject ResultData;

    #region Singleton
    private static SceneChange sc;
    public static SceneChange SC
    {
        get { return sc; }
    }

    private void Awake()
    {
        sc = GetComponent<SceneChange>();
    }
    #endregion

    public void Click_Original() //Original Scene load
    {
        SceneManager.LoadScene("Original");
    }

    public void Click_Dynavision() //Dynavision Scene load
    {
        SceneManager.LoadScene("Dynavision");
    }

    public void Restart() //재시작 Original
    {
        SceneManager.LoadScene("Original");
        Time.timeScale = 1f;
    }

    public void RestartDy() //재시작 Dynavision
    {
        SceneManager.LoadScene("Dynavision");
        Time.timeScale = 1f;
    }

    public void Exit() //나가기
    {
        Application.Quit();
    }

    public void SetType(int num) //1.시선만 2.브레이크만 3. 시선+break 다
    {
        SceneManager.LoadScene("Dynavision");
        GameManager.type = num;
    }

    public void RightPage() //결과화면 오른쪽으로 넘어가기
    {
        Debug.Log("눌림");
        ResultData.transform.localPosition = new Vector3(-1920f, 0f, 0f);
    }

    public void LeftPage() //결과화면 왼쪽으로 넘어가기
    {
        ResultData.transform.localPosition = new Vector3(0f, 0f, 0f);
    }


}
