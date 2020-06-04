using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
public class GazeEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        //스페이스 키 누르면
        if (Input.GetKeyDown("space") && GameManager.GM.resultTime == 0)
        {
            Debug.Log(GameManager.GM.time);
            GameManager.GM.resultTime = GameManager.GM.time;

            GameManager.GM.ViewResult();
        }

    }

    private void OnTriggerEnter(Collider other) //최초 시선 측정
    {
        //공이 나타나면 시선 측정하는데
        if (other.name == "Sphere" && GameManager.GM.see_time == 0)
        {
            GameManager.GM.see_time = GameManager.GM.time;
            Debug.Log("공맞음 " + GameManager.GM.see_time);
        }       
    }
}
