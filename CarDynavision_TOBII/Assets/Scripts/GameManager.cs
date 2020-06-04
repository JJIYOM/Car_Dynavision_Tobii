using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Build Setting
    /// 0 : Select
    /// 1 : Original
    /// 2 : Dynavision
    /// </summary>
    /// 
    public Material Red;
    public Material White;

    public bool timerOn = false;
    public bool lightOn = false;
    public bool IsSixty = false;
    public float time = 0;
    public float see_time = 0; //시선
    public float resultTime = 0; //space 누른 시간
    public float sum = 0f; //시선 합계
    public float Bsum = 0f; //break 합계 
    public int randomNum;
    public static int type = 3;

    public GameObject Sphere;
    public GameObject Spheres;
    public Text EyeText;
    public Text timeText;
    public Text EarlyText;
    public Text eAllText; //eye (시선+break)
    public Text tAllText; //time (시선+break)
    public GameObject ResultPanel;
    public List<float> SeeData;
    public List<float> BreakData;
    public List<int> Overlap;
    //스크립트 수정
    public float CarSpeed;
    bool FirstLightIn60 = true;

    private static GameManager gm;
    public static GameManager GM
    {
        get { return gm; }
    }

    private void Awake()
    {
        gm = GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("StopSign");   
    }

    // Update is called once per frame
    void Update()
    {
        // CarSpeed = CarController.GetInstance().Carspeed;
        CarSpeed = CarController.Carcontroller.Carspeed;
        //Debug.Log(CarSpeed);
        //시간 측정

        if (timerOn)
        {
            time += Time.deltaTime;
        }

        if (CarSpeed >= 60) //차량속도가 60km/h  이상이면
            IsSixty = true;
        else
        {
            IsSixty = false;
            FirstLightIn60 = true;
        }

        //앞으로가기 (연동 후, 엑셀로 수정 예정, w키 여러번 누르면 여러번 실행되는 단점 Dynavision은 bool로 조정하기)
        if (Input.GetKeyDown("w") && lightOn == false)
        {
            if (SceneManager.GetActiveScene().name == "Original")
            {
                StartCoroutine("StopSign");
                Debug.Log("앞으로가기");
                lightOn = true;
            }

            //else if (SceneManager.GetActiveScene().name == "Dynavision") //간단한 코드로 바꿀것
            //    StartCoroutine("StopSignDy");

        }

        //앞으로가기 (연동 후, 엑셀로 수정 예정, w키 여러번 누르면 여러번 실행되는 단점 Dynavision은 bool로 조정하기)
        //차량 속도가 60km/h 이상일 때
        if (IsSixty && !lightOn && (SceneManager.GetActiveScene().name == "Dynavision"))
        {
            Debug.Log("앞으로가기");

            StartCoroutine("StopSignDy");
        }
    }

    //랜덤시간 이후 정지 오브젝트 활성화
    IEnumerator StopSign()
    {
        yield return new WaitForSeconds(Random.Range(1f, 10f));
        Debug.Log("멈춰!");
        Sphere.gameObject.SetActive(true);
        timerOn = true;
    }

    IEnumerator StopSignDy() //불 키는거...
    {
        //이전
        //dynavision : 10번 반복
        //랜덤(3초까지)

        //변경
        //12번 반복, 60km/h 도달시, 총 대기시간 5초
        if (Overlap.Count < 12)//SeeData.Count < 12 && BreakData.Count < 12)
        {
            if (FirstLightIn60)
            {
                //if (Overlap.Count > 0)
                //{
                //    CarR2 temp2 = new CarR2();
                //    temp2.StayTime = DynaGazeEvent.DGE.stayTime.ToString();
                //    Car_Result.CarR2.Add(temp2);
                //    Debug.Log(DynaGazeEvent.DGE.stayTime);
                //}

                FirstLightIn60 = false;
                Debug.Log("Dyna멈춰!");
                randomNum = Random.Range(0, 54);
                Overlap.Add(randomNum);//랜덤 나온거 추가하고
                Spheres.transform.GetChild(randomNum).gameObject.SetActive(true); //총 33개(0~32)   //총 54개로 변경
                timerOn = true;
                lightOn = true;
                DynaGazeEvent.DGE.FirstLightOn = true;

                yield return new WaitForSeconds(5f);

                if (time > 5 && (lightOn == true)) //5초 안보면
                {                   
                    if (DynaGazeEvent.DGE.IsSeeR == false)
                        SeeData.Add(0);
                    if (DynaGazeEvent.DGE.IsBreak == false)
                        BreakData.Add(0);
                    //StartCoroutine("LightOn");
                    DynaGazeEvent.DGE.IsSeeR = false;
                    DynaGazeEvent.DGE.AddSaveXResult();
                    LightOff();
                }
            }
        }
        else //if (SeeData.Count == 12 || BreakData.Count == 12) //둘중 하나 만족하면
        {         
            lightOn = true;
            Debug.Log("끝났덩");
            ViewResult();
        }
    }

    public void LightOff()
    {
        //Spheres.transform.GetChild(randomNum).gameObject.SetActive(false);
        DynaGazeEvent.DGE.IsTimeOver = true; //타임오버야~

        //여기에 초기화하는 함수 실행 + 보는거에 초기화하는 함수도 실행
        DynaGazeEvent.DGE.InitMenu();

        //saveCarXResult();
    }

    ////깜빡 2번(2초)해도 안보면 다음으로 넘어가
    //IEnumerator LightOn()
    //{
    //    Debug.Log("깜빡깜빡");

    //    for (int i = 0; i < 2; i++)
    //    {
    //        Spheres.transform.GetChild(randomNum).GetComponent<MeshRenderer>().material = White;
    //        yield return new WaitForSeconds(0.5f);
    //        Spheres.transform.GetChild(randomNum).GetComponent<MeshRenderer>().material = Red;
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //    //Spheres.transform.GetChild(randomNum).gameObject.SetActive(false);
    //    DynaGazeEvent.DGE.IsTimeOver = true; //타임오버야~

    //    //여기에 초기화하는 함수 실행 + 보는거에 초기화하는 함수도 실행
    //    DynaGazeEvent.DGE.InitMenu();

    //    saveCarXResult();
    //}

    public void ViewResult() //결과 보여주는 함수
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1: //Original
                if (timerOn == false) //구 활성화 전 정지
                {
                    timeText.gameObject.SetActive(false);
                    EyeText.gameObject.SetActive(false);
                    EarlyText.gameObject.SetActive(true);
                }
                else if (see_time == 0)
                {
                    EyeText.text = string.Format("정지 신호를 보지 않음");
                    timeText.text = string.Format("반응 속도 : {0:N2}초", resultTime);
                }
                else
                {
                    EyeText.text = string.Format("시선 측정 : {0:N2}초", see_time); /*"시선 측정 : " + see_time + "초"*/
                    timeText.text = string.Format("반응 속도 : {0:N2}초", resultTime); /*"반응 속도 : " + resultTime + "초";*/
                }
                ResultPanel.SetActive(true); //패널 ON
                Time.timeScale = 0f;

                break;
            case 2: //Dynavision
                    //시선 데이터(list : Overlap) 평균시간 출력하기 - 수정하기
                    //1.시선, 2. 반응속도 3. 시선 + 반응속도 둘다

                ResultPanel.SetActive(true); //패널 ON
                Time.timeScale = 0f;

                for (int i = 0; i < SeeData.Count; i++)
                {
                    sum += SeeData[i];
                }
                for (int i = 0; i < BreakData.Count; i++)
                {
                    Bsum += BreakData[i];
                }

                float total = (sum / SeeData.Count);
                float Btotal = (Bsum / BreakData.Count);

                switch (type)
                {
                    case 1: //시선
                        EyeText.text = string.Format("평균 시간(시선) : {0:N3}초", total);
                        EyeText.gameObject.SetActive(true);
                        break;
                    case 2: //break
                        timeText.text = string.Format("반응 시간 : {0:N3}초", Btotal);
                        timeText.gameObject.SetActive(true);
                        break;
                    case 3:
                        eAllText.text = string.Format("시선 측정 : {0:N3}초", total);
                        tAllText.text = string.Format("반응 시간 : {0:N3}초", Btotal);
                        eAllText.gameObject.SetActive(true);
                        tAllText.gameObject.SetActive(true);
                        break;
                }

                break;
        }
    }

    //public void saveCarXResult()
    //{
    //    CarR temp = new CarR(); //틀림, 반응시간 추가...
    //    temp.Response_Time = "0";
    //    temp.O_X = "X";
    //    Car_Result.CarR.Add(temp);
    //}

   
}
