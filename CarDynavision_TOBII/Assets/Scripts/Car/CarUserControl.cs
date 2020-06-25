using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

/*
 * Logitech - 자동차 오브젝트 연결 스크립트
 */

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        float handle_Input;
        float accel_Input;
        float m_SteeringAngle;
        public float break_Input;

        public float MaxSteerAngle;
        public float motorForce;
        public float breakForce;
        //public List<float> motor_torque = new List<float>();
        //public List<float> brake_torque = new List<float>();
        public Transform Handle;
        public Transform Rpm_Pointer;
        public Transform Speed_Pointer;

        private CarController m_Car; // the car controller we want to use

        int drivingMode = 0;

        #region singleton
        private static CarUserControl instance = null;
        public static CarUserControl Instance
        {
            get { return instance; }
        }
        #endregion


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                return;
            }
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");

            GetInput(); //logitech 값 받아오기(변환)

            Steer(); //핸들 회전    
            RPM(); //계기판, rpm
            CarSpeed(); //속도계         

            //자동차 이동 관련

            float h = handle_Input;
            float v = accel_Input;
            //float b = break_Input;
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            if (drivingMode == -1) //drive시 이동
                m_Car.Move(h, v, v, handbrake);
            else if (drivingMode == 1) //후진
                m_Car.Move(h, -v, -v, handbrake);//Move(float steering, float accel, float footbrake, float handbrake)
            else //중립
            { }
#else
            m_Car.Move(h, v, v, 0f);
            
#endif
            if (break_Input > 0) //브레이크를 밟을 때
            {

                for (int i = 0; i < 4; i++) //바퀴4개
                {
                    CarController.Carcontroller.m_WheelColliders[i].motorTorque = 0f; //모터 토크 값 설정
                    CarController.Carcontroller.m_WheelColliders[i].brakeTorque = break_Input * 100000f; //브레이크 토크 값 설정 (끼익 하고 빠르게 급정거 할 수 있게)
                    //m_Car.Move(h, v, v, handbrake);
                }
            }
            else //안밟았을 때
            {
                if (CarController.Carcontroller.CurrentSpeed <= 0.0001f)
                {
                    for (int i = 0; i < 4; i++) //바퀴4개
                    {
                        CarController.Carcontroller.m_WheelColliders[i].motorTorque = 1f;
                        CarController.Carcontroller.m_WheelColliders[i].brakeTorque = 0f;
                    }
                }
            }
        }

        public void GetInput() //logitech 데이터 변환
        {
            //raw 값 받아오기
            LogitechGSDK.DIJOYSTATE2ENGINES recs = LogitechGSDK.LogiGetStateUnity(1);

            Debug.Log(recs);

            //변환(Demo 프로그램보면서 값 찾기)
            handle_Input = (recs.lX / 32768f);
            accel_Input = (1 - (recs.lY / 32767f)) / 2;
            break_Input = (1 - (recs.lRz / 32767f)) / 2;

            // -1 : 전진
            // 0 : 중립
            // 1 : 후진

            // 기어를 위로 올리면 전진
            if (recs.rgbButtons[14] == 128)
            {
                drivingMode = -1;
            }

            // 기어를 아래로 내리면 후진
            else if (recs.rgbButtons[15] == 128)
            {
                drivingMode = 1;
            }

            else //중립
            {
                drivingMode = 0;
            }
        }

        // 핸들의 회전량에 차량 내부의 핸들 변화 & 핸들 회전량에 따른 바퀴의 회전 값 설정
        void Steer()
        {
            m_SteeringAngle = MaxSteerAngle * handle_Input;

            //핸들 회전값을 로지텍 핸들 회전값과 동일화
            Handle.localRotation = Quaternion.Euler(0, 0, -handle_Input * 420f);
        }

        void RPM() //rpm 포인터 회전값 설정
        {
            float rpm = 0f;

            for (int i = 0; i < 4; i++) //바퀴가 4개니까
            {
                rpm += CarController.Carcontroller.m_WheelColliders[i].rpm;
                //Debug.Log(CarController.Carcontroller.m_WheelColliders[i].rpm);
            }

            rpm = rpm / 4; //rpm 평균 내기

            //Debug.Log(rpm);
            // 차량 내부에 있는 RPM Pointer를 Accel의 세기에 따라 변환 
            if (rpm < 0)
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, 0);
            else if (rpm > 900)
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, -240f);
            else
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, -rpm / 1000f * 240f);
        }

        void CarSpeed() //차량 속도 포인터 회전값 설정
        {
            float speed = 0f;
            //Debug.Log(CarController.Carcontroller.CurrentSpeed * 1.609344);
            speed = CarController.Carcontroller.CurrentSpeed;

            if (speed < 0)
                Speed_Pointer.localRotation = Quaternion.Euler(0, 0, 0);
            else if (speed > 200)
                Speed_Pointer.localRotation = Quaternion.Euler(0, 0, -240f);
            else
                Speed_Pointer.localRotation = Quaternion.Euler(0, 0, -speed / 200 * 240f);
        }
    }
}