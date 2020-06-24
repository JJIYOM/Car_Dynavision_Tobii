using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 차량 데이터 (CSV 항목)
 */

public class Car_Data //첫번째 CSV (Saved_data_Car)
{
    public static List<Car> Car = new List<Car>();
    public static List<Car2> Car2 = new List<Car2>();

    public static string getPath() //저장경로
    {
#if UNITY_EDITOR 
        return Application.dataPath + "/CSV/" + "Saved_data_Car.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"/Saved_data_Car.csv";
#elif UNITY_STANDALONE_WIN
        return Application.persistentDataPath+"/"+"Saved_data_Car.csv";
#endif
    }
}

public class Car //첫 번째 항목 1
{
    public string t_Time { get; set; } //플레이시간 (실시간)
    public string EyeLocation_x { get; set; } //Gaze x좌표 (실시간)
    public string EyeLocation_y { get; set; } //Gaze y좌표 (실시간)
    public string HeadPose_x { get; set; } //HeadPose x좌표 (실시간)
    public string HeadPose_y { get; set; } //HeadPose y좌표 (실시간)
    public string HeadPose_z { get; set; } //HeadPose z좌표 (실시간)
    public string CheckPoint { get; set; } //최초 시선 반응
}

public class Car2 //첫 번째 항목 2
{
    public string LightOn { get; set; } //공이 켜졌을 때
}

public class Car_Result //두 번째 CSV (Car_Result)
{
    public static List<CarR> CarR = new List<CarR>();

    public static string getPath() //저장경로
    {
#if UNITY_EDITOR 
        return Application.dataPath + "/CSV/" + "Car_Result.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"/Car_Result.csv";
#elif UNITY_STANDALONE_WIN
        return Application.persistentDataPath+"/"+"Car_Result.csv";
#endif
    }
}

public class CarR //두 번째 항목들
{
    public string Stage { get; set; } //단계 (총 12번)
    public string Response_Time { get; set; } //시선 반응 시간
    public string Brake_Time { get; set; } //브레이크 반응 시간
    public string O_X { get; set; } //성공 유무
    public string BallNum { get; set; } //본 공 번호
}
