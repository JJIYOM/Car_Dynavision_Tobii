using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;


//시선+break 이벤트 처리 스크립트
public class DynaGazeEvent : MonoBehaviour
{
    public int Stage = 0;
    public float stayTime = 0;
    public bool IsSee = false;
    public bool IsSeeR = false;
    public bool IsBreak = false;
    public bool IsTimeOver = false;
    public bool FirstLightOn = false;
    public float T_time = 0f;

    private static DynaGazeEvent dge;
    public static DynaGazeEvent DGE
    {
        get { return dge; }
    }

    private void Awake()
    {
        dge = GetComponent<DynaGazeEvent>();
    }


    // Start is called before the first frame update
    void Start()
    {    
        
    }

    public void OnApplicationQuit()
    {
        UpdateCarFile();
        UpdateCarFileR();
    }

    // Update is called once per frame
    void Update()
    {
        T_time += Time.deltaTime;

        if (IsSeeR) //이름이 동일하고 본거 체크 되었다면
        {
            stayTime += Time.deltaTime;
        }

        saveCarData();

        ////스페이스 키 누르면(여러번 실행되는 현상)
        ////구가 생성된 상태에서만 실행할 수 있게 수정할 것
        if (UnityStandardAssets.Vehicles.Car.CarUserControl.Instance.break_Input > 0.03 && GameManager.GM.timerOn == true && IsSeeR == true)
        {
            GameManager.GM.resultTime = GameManager.GM.time;
            GameManager.GM.BreakData.Add(GameManager.GM.resultTime); //리스트에 break데이터 넣기
            Debug.Log("break 버튼누름 " + GameManager.GM.resultTime);
            IsBreak = true;
            AddSaveOResult();

            InitMenu();
        }

    }

    private void OnTriggerEnter(Collider other) //최초 시선 측정
    {
        Debug.Log(other.name);
        //공이 나타나면 시선 측정하는데
        if (other.name == "Sphere" && GameManager.GM.see_time == 0)
        {
            GameManager.GM.see_time = GameManager.GM.time;
            GameManager.GM.SeeData.Add(GameManager.GM.see_time); //리스트에 시선데이터 넣기

            //CarR temp = new CarR();
            //temp.Response_Time = GameManager.GM.see_time;
            //temp.O_X = "O";
            //Car_Result.CarR.Add(temp);

            Debug.Log("Dyna공맞음 " + GameManager.GM.see_time);
            IsSee = true;
            IsSeeR = true;
            

            //InitMenu();
        }
    }

    private void OnTriggerStay(Collider other) //응시(우선 1초)
    {
        #region 주석
        //Debug.Log(other.gameObject.name);

        /* 
        if (other.gameObject.name == GameManager.GM.RandomName) //빨간 공 쳐다보면
        {
             time += Time.deltaTime;

             //여기서 Time이 1초 넘어가면
             if (time >= 1)
             {
                 //여기에 O들어가야대고...
                 Debug.Log("1초 응시완료");
                 other.GetComponent<MeshRenderer>().material = GameManager.GM.White;
                 GameManager.GM.StartCoroutine("RandomColor");
                 UIManager.UI._stage++;
                 if (UIManager.UI._stage != 31) UIManager.UI.ViewStage();
                 time = 0f;
             }
         }
         */
        #endregion

        if (other.gameObject.name == GameManager.GM.randomNum.ToString() && IsSeeR) //이름이 동일하고 본거 체크 되었다면
        {
            stayTime += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == GameManager.GM.randomNum.ToString())
            IsSeeR = false;
    }

    public void InitMenu()
    {
        //초기화 조건 3가지로 나눌껀데 시작 전 버튼 클릭으로 parameter 받아서 1,2,3으로 나누기

        // dynavision 3가지 구별
        /* 1. 시선만 처리
         * 2. break만 처리
         * 3. 시선 + break 처리 -> 현재는 이걸로 진행
        */

        //공통되는 초기화 조건은 5초가 지났을 때.(타임오버일때)

        switch (GameManager.type)
        {
            case 1: //IsSee == true이면 초기화
                if (IsSee == true || IsTimeOver)
                {
                    Init();
                }
                break;

            case 2: //IsBreak == true이면 초기화
                if (IsBreak == true || IsTimeOver)
                {
                    Init();
                }
                break;

            case 3: //IsSee == true && IsBreak == true이면 초기화
                if ((IsSeeR == true && IsBreak == true) || IsTimeOver)
                {
                    Init();
                }
                break;
        }
    }

    public void Init() //초기화(시간,공,break)
    {
        GameManager.GM.Spheres.transform.GetChild(GameManager.GM.randomNum).gameObject.SetActive(false);

        GameManager.GM.time = 0f;
        GameManager.GM.see_time = 0f;
        GameManager.GM.resultTime = 0f;
        GameManager.GM.timerOn = false;
        GameManager.GM.lightOn = false;
        IsTimeOver = false;
        IsSeeR = false;
        IsBreak = false;
    }

    public void saveCarData()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        Car temp = new Car();
        temp.t_Time = T_time.ToString();
        temp.EyeLocation_x = (gazePoint.Screen.x - 12 - 960).ToString();
        temp.EyeLocation_y = (gazePoint.Screen.y - 12 - 540).ToString();
        temp.HeadPose_x = HeadMovement.HM.HeadRotation.x.ToString();
        temp.HeadPose_y = HeadMovement.HM.HeadRotation.y.ToString();
        temp.HeadPose_z = HeadMovement.HM.HeadRotation.z.ToString();


        // Car_Data.Car.Add(temp);

        if (!IsSee && !IsTimeOver) //불켜지기 전
            temp.CheckPoint = "";
        else if (!IsSee && IsTimeOver) //5초동안 보지 않고 불 꺼지면
            temp.CheckPoint = "X";
        else if (IsSee)//불 켜졌을때 봤으면
            temp.CheckPoint = GameManager.GM.randomNum.ToString();

        Car2 temp2 = new Car2();

        if (FirstLightOn)
        {
            temp2.LightOn = GameManager.GM.randomNum.ToString();
            FirstLightOn = false;
        }
        else
        {
            temp2.LightOn = "";
        }

        //Debug.Log(temp.EyeLocation_x + "        " + temp.EyeLocation_y);
        Car_Data.Car.Add(temp);
        Car_Data.Car2.Add(temp2);

        IsSee = false;

    }

    public void AddSaveOResult()
    {
        Stage++;
        CarR temp = new CarR();

        temp.Stage = ""+Stage;
        temp.Response_Time = "" + GameManager.GM.see_time;
        temp.Brake_Time = "" + GameManager.GM.resultTime;
        temp.O_X = "O";
        temp.BallNum = GameManager.GM.Overlap[Stage - 1].ToString();
        
        //temp.StayTime = stayTime.ToString();
        Car_Result.CarR.Add(temp);

        stayTime = 0f;
    }

    public void AddSaveXResult()
    {
        Stage++;
        CarR temp = new CarR(); //틀림, 반응시간 추가...

        temp.Stage = "" + Stage;
        temp.Response_Time = "" + 0;
        temp.Brake_Time = "" + 0;
        temp.O_X = "X";
        temp.BallNum = GameManager.GM.Overlap[Stage-1].ToString();

        Car_Result.CarR.Add(temp);
    }

    void UpdateCarFile()
    {
        string filePath = Car_Data.getPath();
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine("Time,EyeLocation_x,EyeLocation_y,HeadPose_x,HeadPose_y,HeadPose_z,CheckPoint,LightOn");
        for (int i = 0; i < Car_Data.Car.Count; i++)
        {
            string str = Car_Data.Car[i].t_Time + "," + Car_Data.Car[i].EyeLocation_x + "," + Car_Data.Car[i].EyeLocation_y + "," + Car_Data.Car[i].HeadPose_x + "," + Car_Data.Car[i].HeadPose_y + "," + Car_Data.Car[i].HeadPose_z + "," + Car_Data.Car[i].CheckPoint + "," + Car_Data.Car2[i].LightOn;
            outStream.WriteLine(str);
        }

        outStream.Close();
    }

    void UpdateCarFileR()
    {
        string filePath = Car_Result.getPath();
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine("Stage,Response_Time,Brake_Time,O_X,BallNum");
        for (int i = 0; i < Car_Result.CarR.Count; i++)
        {
            string str = Car_Result.CarR[i].Stage + "," + Car_Result.CarR[i].Response_Time + "," + Car_Result.CarR[i].Brake_Time + "," + Car_Result.CarR[i].O_X + ","  + Car_Result.CarR[i].BallNum;

            outStream.WriteLine(str);
        }

        outStream.Close();
    }

    //dynavision
    //공 보고있는 상태로 space 누르면 공 꺼지고 다음 공 나오게
}
