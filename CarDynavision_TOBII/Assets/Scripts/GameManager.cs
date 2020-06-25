using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

/*
 * 게임 매니저 스크립트 
 */

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Build Setting
    /// 0 : Select
    /// 1 : Original
    /// 2 : Dynavision
    /// </summary>

    #region 변수
    public Material Red;
    public Material White;

    public bool timerOn = false;
    public bool lightOn = false;
    public bool IsSixty = false;
    public float time = 0;
    public float see_time = 0; //최초 시선 반응 시간
    public float resultTime = 0; //space 누른 시간
    public float sum = 0f; //시선 합계
    public float Bsum = 0f; //break 합계 
    public int randomNum;
    public static int type = 3;

    public GameObject Sphere;
    public GameObject Spheres;
    public GameObject RightPage;
    public GameObject LeftPage;
    public Text EyeText;
    public Text timeText;
    public Text EarlyText;
    public Text[] ResponseTime;
    public Text[] BrakeTime;

    public GameObject ResultPanel;
    public List<float> SeeData; //최초 시선 반응시간 데이터 저장 리스트
    public List<float> BreakData;
    public List<int> Overlap;
    //스크립트 수정
    public float CarSpeed;
    bool FirstLightIn60 = true; //처음에는 그냥 이벤트 실행되어야 하니까 true로 설정 //60 아래를 찍고왔는지 여부
    #endregion

    #region Singleton
    private static GameManager gm;
    public static GameManager GM
    {
        get { return gm; }
    }

    private void Awake()
    {
        gm = GetComponent<GameManager>();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("StopSign");   
    }

    // Update is called once per frame
    void Update()
    {
        // CarSpeed = CarController.GetInstance().Carspeed;
        CarSpeed = CarController.Carcontroller.Carspeed; //차량 속도 받아오기
        //Debug.Log(CarSpeed);
        //시간 측정

        if (timerOn) //타이머가 켜지면 시간 측정 시작
        {
            time += Time.deltaTime;
        }

        if (CarSpeed >= 60) //차량속도가 60km/h  이상이면
            IsSixty = true;

        else //60km/h 아래면
        {
            IsSixty = false;
            FirstLightIn60 = true; //60km/h 아래를 찍었다!
        }

        //차량 속도가 60km/h 이상일 때
        if (IsSixty && !lightOn && (SceneManager.GetActiveScene().name == "Dynavision")) //60km/h 이상이고, 불이 안켜진 상태, Dynavision 씬일 경우
        {
            Debug.Log("앞으로가기");

            StartCoroutine("StopSignDy"); //불 키는 이벤트 함수 실행
        }

        //앞으로가기 (연동 후, 엑셀로 수정 예정, w키 여러번 누르면 여러번 실행되는 단점 Original)
        if (Input.GetKeyDown("w") && lightOn == false) //original
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
    }

    //랜덤시간 이후 정지 오브젝트 활성화
    IEnumerator StopSign() //Original
    {
        yield return new WaitForSeconds(Random.Range(1f, 10f));
        Debug.Log("멈춰!");
        Sphere.gameObject.SetActive(true);
        timerOn = true;
    }

    IEnumerator StopSignDy() //불 키는거 (Dynavision)
    {
        //이전
        //dynavision : 10번 반복
        //랜덤(3초까지)

        //변경
        //12번 반복, 60km/h 도달시, 총 대기시간 5초
        if (Overlap.Count < 12) //게임 이벤트 (12번)
        {
            if (FirstLightIn60) //60km/h 아래를 찍고왔으면 이벤트 실행
            {
                //if (Overlap.Count > 0)
                //{
                //    CarR2 temp2 = new CarR2();
                //    temp2.StayTime = DynaGazeEvent.DGE.stayTime.ToString();
                //    Car_Result.CarR2.Add(temp2);
                //    Debug.Log(DynaGazeEvent.DGE.stayTime);
                //}

                FirstLightIn60 = false; //다시 60km/h 아래를 찍어야댐 => false

                Debug.Log("Dyna멈춰!");
                randomNum = Random.Range(0, 69); //공 개수 총 69개
                Overlap.Add(randomNum);//켜지는 공 번호 추가
                Spheres.transform.GetChild(randomNum).gameObject.SetActive(true); //공 켜주기

                timerOn = true; //불이 켜지면 타이머On
                lightOn = true; //불이 켜졌음

                DynaGazeEvent.DGE.FirstLightOn = true;

                yield return new WaitForSeconds(5f);

                if (time > 5 && (lightOn == true)) //5초 안보면
                {
                    //데이터 0으로 넣어주기
                    if (DynaGazeEvent.DGE.IsSeeR == false)
                        SeeData.Add(0);
                    if (DynaGazeEvent.DGE.IsBreak == false)
                        BreakData.Add(0);

                    DynaGazeEvent.DGE.IsSeeR = false;

                    //CSV에 넣어주기 (X 버전)
                    DynaGazeEvent.DGE.AddSaveXResult();
                    LightOff();
                }
            }
        }
        else //12번 반복 끝나면
        {
            lightOn = true;
            Debug.Log("끝났덩");
            ViewResult(); //결과화면 보여주기
        }
    }

    public void LightOff() //불 꺼주는 함수(5초동안 아무것도 안했을 경우)
    {
        DynaGazeEvent.DGE.IsTimeOver = true; //타임오버

        //초기화
        DynaGazeEvent.DGE.InitMenu();
    }

    public void ViewResult() //결과 보여주는 함수
    {
        switch (SceneManager.GetActiveScene().buildIndex) //씬에 따라
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
                    EyeText.text = string.Format("시선 측정 : {0:N2}초", see_time);
                    timeText.text = string.Format("반응 속도 : {0:N2}초", resultTime);
                }
                ResultPanel.SetActive(true); //패널 ON
                Time.timeScale = 0f;

                break;

            case 2: //Dynavision
                    //시선 데이터(list : Overlap) 평균시간 출력하기
                    //1.시선, 2. 반응속도 3. 시선 + 반응속도 둘다

                ResultPanel.SetActive(true); //패널 ON
                Time.timeScale = 0f;

                for (int i = 0; i < SeeData.Count; i++) //총 12회
                {
                    ResponseTime[i].text = string.Format("{0:N3}", SeeData[i]); //시선 값을 소수점 3자리로 출력              
                    sum += SeeData[i]; //총 반응시간 합계
                }
                for (int i = 0; i < BreakData.Count; i++) //총 12회
                {
                    BrakeTime[i].text = string.Format("{0:N3}", BreakData[i]); //브레이크 값을 소수점 3자리로 출력      
                    Bsum += BreakData[i]; //총 반응시간 합계
                }

                float total = (sum / SeeData.Count); //평균 구하기
                float Btotal = (Bsum / BreakData.Count); //평균 구하기

                //평균시간 & 패널 관리
                switch (type)
                {
                    case 1: //시선
                        EyeText.text = string.Format("평균 시선 반응 시간 : {0:N3}초", total); //시선 평균시간                     
                        RightPage.gameObject.SetActive(false); //이동 버튼 비활성화
                        break;
                    case 2: //break
                        timeText.text = string.Format("평균 시간 : {0:N3}초", Btotal); //브레이크 평균시간  
                        SceneChange.SC.RightPage();
                        LeftPage.gameObject.SetActive(false); //이동 버튼 비활성화
                        break;
                    case 3: //시선+브레이크
                        EyeText.text = string.Format("평균 시선 반응 시간 : {0:N3}초", total);
                        timeText.text = string.Format("평균 시간 : {0:N3}초", Btotal);

                        break;
                }

                break;
        }
    }


}
