using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Data
{
    public static List<Car> Car = new List<Car>();
    public static List<Car2> Car2 = new List<Car2>();

    public static string getPath()
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

public class Car
{
    public string t_Time { get; set; }
    public string EyeLocation_x { get; set; }
    public string EyeLocation_y { get; set; }
    public string HeadPose_x { get; set; }
    public string HeadPose_y { get; set; }
    public string HeadPose_z { get; set; }
    public string CheckPoint { get; set; }
}

public class Car2
{
    public string LightOn { get; set; }
}

public class Car_Result
{
    public static List<CarR> CarR = new List<CarR>();

    public static string getPath()
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

public class CarR
{
    public string Stage { get; set; }
    public string Response_Time { get; set; }
    public string Brake_Time { get; set; }
    public string O_X { get; set; }
    public string BallNum { get; set; }
}
