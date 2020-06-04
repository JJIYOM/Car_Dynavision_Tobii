using UnityEngine;
using System.Collections;
using System.Text;

public class LogitechSteeringWheel : MonoBehaviour
{

    LogitechGSDK.LogiControllerPropertiesData properties;

    // Use this for initialization
    void Start()
    {
        //Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
        LogitechGSDK.LogiSteeringInitialize(false);
        Debug.Log(LogitechGSDK.LogiIsConnected(1));
        LogitechGSDK.LogiPlaySpringForce(0, 0, 20, -40);
        LogitechGSDK.LogiPlaySpringForce(1, 0, 20, 40);
        //LogitechGSDK.LogiPlayConstantForce(0, -100);
        //LogitechGSDK.LogiPlayConstantForce(1, -100);
    }



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(LogitechGSDK.LogiPlaySpringForce(0, 0, 0, -100) + "  " + LogitechGSDK.LogiPlaySpringForce(1,0,0,-100) + "   " + LogitechGSDK.LogiPlayConstantForce(0, -100)+"   "+ LogitechGSDK.LogiPlayConstantForce(1, -100));

        //All the test functions are called on the first device plugged in(index = 0)
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(1))
        {
            Debug.Log("연결");
            LogitechGSDK.DIJOYSTATE2ENGINES recs = LogitechGSDK.LogiGetStateUnity(1);

            // 첫번째는 기기 , 두번째는 스프링 중심값, 세번째는 스프링 강도, 네번째는 편향
            LogitechGSDK.LogiPlaySpringForce(0, 0, 20, -40);
            LogitechGSDK.LogiPlaySpringForce(1, 0, 20, 40);

            //Debug.Log("핸들 :" + recs.lX);
            //Debug.Log("액셀 :" + recs.lY);
            //Debug.Log("브레이크 :" + recs.lRz);
            //Debug.Log("패달 클러치 :" + recs.rglSlider[0]);


            //if (recs.rgbButtons[4] == 128) Debug.Log("우측 핸들 변속기 가동");
            //else if (recs.rgbButtons[5] == 128) Debug.Log("좌측 핸들 변속기 가동");
            //else if (recs.rgbButtons[12] == 128) Debug.Log("좌상 변속기 가동");
            //else if (recs.rgbButtons[13] == 128) Debug.Log("좌하 변속기 가동");
            //else if (recs.rgbButtons[14] == 128) Debug.Log("중상 변속기 가동");
            //else if (recs.rgbButtons[15] == 128) Debug.Log("중하 변속기 가동");
            //else if (recs.rgbButtons[16] == 128) Debug.Log("우상 변속기 가동");
            //else if (recs.rgbButtons[17] == 128) Debug.Log("우하 변속기 가동");

        }
    }
    void OnApplicationQuit()
    {
        Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }



}
