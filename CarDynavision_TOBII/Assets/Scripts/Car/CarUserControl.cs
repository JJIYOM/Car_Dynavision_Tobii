using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

/*
 * Logitech - �ڵ��� ������Ʈ ���� ��ũ��Ʈ
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

            GetInput(); //logitech �� �޾ƿ���(��ȯ)

            Steer(); //�ڵ� ȸ��    
            RPM(); //�����, rpm
            CarSpeed(); //�ӵ���         

            //�ڵ��� �̵� ����

            float h = handle_Input;
            float v = accel_Input;
            //float b = break_Input;
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            if (drivingMode == -1) //drive�� �̵�
                m_Car.Move(h, v, v, handbrake);
            else if (drivingMode == 1) //����
                m_Car.Move(h, -v, -v, handbrake);//Move(float steering, float accel, float footbrake, float handbrake)
            else //�߸�
            { }
#else
            m_Car.Move(h, v, v, 0f);
            
#endif
            if (break_Input > 0) //�극��ũ�� ���� ��
            {

                for (int i = 0; i < 4; i++) //����4��
                {
                    CarController.Carcontroller.m_WheelColliders[i].motorTorque = 0f; //���� ��ũ �� ����
                    CarController.Carcontroller.m_WheelColliders[i].brakeTorque = break_Input * 100000f; //�극��ũ ��ũ �� ���� (���� �ϰ� ������ ������ �� �� �ְ�)
                    //m_Car.Move(h, v, v, handbrake);
                }
            }
            else //�ȹ���� ��
            {
                if (CarController.Carcontroller.CurrentSpeed <= 0.0001f)
                {
                    for (int i = 0; i < 4; i++) //����4��
                    {
                        CarController.Carcontroller.m_WheelColliders[i].motorTorque = 1f;
                        CarController.Carcontroller.m_WheelColliders[i].brakeTorque = 0f;
                    }
                }
            }
        }

        public void GetInput() //logitech ������ ��ȯ
        {
            //raw �� �޾ƿ���
            LogitechGSDK.DIJOYSTATE2ENGINES recs = LogitechGSDK.LogiGetStateUnity(1);
           
            Debug.Log(recs);

            //��ȯ(Demo ���α׷����鼭 �� ã��)
            handle_Input = (recs.lX / 32768f);
            accel_Input = (1 - (recs.lY / 32767f)) / 2;
            break_Input = (1 - (recs.lRz / 32767f)) / 2;

            // -1 : ����
            // 0 : �߸�
            // 1 : ����

            // �� ���� �ø��� ����
            if (recs.rgbButtons[14] == 128)
            {
                drivingMode = -1;
            }

            // �� �Ʒ��� ������ ����
            else if (recs.rgbButtons[15] == 128)
            {
                drivingMode = 1;
            }

            else //�߸�
            {
                drivingMode = 0;
            }
        }

        // �ڵ��� ȸ������ ���� ������ �ڵ� ��ȭ & �ڵ� ȸ������ ���� ������ ȸ�� �� ����
        void Steer()
        {
            m_SteeringAngle = MaxSteerAngle * handle_Input;

            //�ڵ� ȸ������ ������ �ڵ� ȸ������ ����ȭ
            Handle.localRotation = Quaternion.Euler(0, 0, -handle_Input * 420f);
        }

        void RPM() //rpm ������ ȸ���� ����
        {
            float rpm = 0f;

            for (int i = 0; i < 4; i++) //������ 4���ϱ�
            {
                rpm += CarController.Carcontroller.m_WheelColliders[i].rpm;
                //Debug.Log(CarController.Carcontroller.m_WheelColliders[i].rpm);
            }

            rpm = rpm / 4; //rpm ��� ����

            //Debug.Log(rpm);
            // ���� ���ο� �ִ� RPM Pointer�� Accel�� ���⿡ ���� ��ȯ 
            if (rpm < 0)
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, 0);
            else if (rpm > 900)
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, -240f);
            else
                Rpm_Pointer.localRotation = Quaternion.Euler(0, 0, -rpm / 1000f * 240f);
        }

        void CarSpeed() //���� �ӵ� ������ ȸ���� ����
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