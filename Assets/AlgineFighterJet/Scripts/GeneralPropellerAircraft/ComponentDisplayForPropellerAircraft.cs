using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algine.GeneraPropellerAircraft;
namespace Algine.Aircraft
{
    public class ComponentDisplayForPropellerAircraft : MonoBehaviour
    {
        #region Aircraft_Orientation
        [SerializeField]
        private Transform m_AircraftParent;
        [SerializeField]
        private RectTransform[] m_AiRoll;
        [SerializeField]
        private RectTransform m_AiPitch;
        [SerializeField]
        private RectTransform m_Yaw;
        [SerializeField]
        private float m_pitchLimit = 49f;
        #endregion
        #region AltMeter
        [SerializeField]
        private RectTransform m_hand_100;
        [SerializeField]
        private RectTransform m_hand_1000;
        [SerializeField]
        private RectTransform m_hand_10000;
        [SerializeField]
        private RectTransform m_pr_inhg;
        [SerializeField]
        private RectTransform m_pr_inmbr;
        #endregion
        #region AirSpeedMeter
        [SerializeField]
        private Transform m_air_speed_rotor;
        [SerializeField]
        private float HightestAirSpeedInNoticlPerHour = 200f;
        [SerializeField]
        private float HighestAngleOfAirMeter = 320f;
        #endregion
        #region VarioMeter
        [SerializeField]
        private Transform m_vario_meter_rotor;
        private float HightestClimbRateInKiloFeetPerMIn = 200f;
        [SerializeField]
        private float HighestAngleOfVarioMeter = 160f;
        
        #endregion

        private const float MeterToFeet = 3.28084f;
        private const float MeterToNaticalMile = 1.944f;
        private const float MeterPerSecToFeetPerMin = 3.28084f *60/100f;

        private Rigidbody m_aircraftRigidbody;

        //previous Sates
        private Vector3 m_lastVelocity = Vector3.zero;

        [SerializeField]
        private bool isFloatingPrecisionEnable = false;

        private double altitude = 0.0f;
        private double offset = 0f;

        private void Start()
        {
            if (m_AircraftParent == null)
            {
                Debug.LogError("Missing Aircraft Parent");
            }
            GameEvents.Current.onAircraftRotationChanged += OnAircraftRotationChanged;
            GameEvents.Current.onOffsetChanged += OnOffsetChanged;

            m_aircraftRigidbody = m_AircraftParent.GetComponent<Rigidbody>();
            m_lastVelocity = m_aircraftRigidbody.velocity;
        }
        private void OnOffsetChanged(Vector3 delta)
        {
            offset += delta.y;
        }
        private void LateUpdate()
        {
            //for AltMeter
            if (isFloatingPrecisionEnable)
            {
                altitude = offset + m_AircraftParent.position.y;
                float heightFeet = (float)altitude * MeterToFeet;
                m_hand_100.rotation = Quaternion.Euler(0, 0, -(heightFeet / 100) * 360);
                m_hand_1000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 1000) * 360);
                m_hand_10000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 10000) * 360);
            }
            else
            {
                float heightFeet = m_AircraftParent.position.y * MeterToFeet;
                m_hand_100.rotation = Quaternion.Euler(0, 0, -(heightFeet / 100) * 360);
                m_hand_1000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 1000) * 360);
                m_hand_10000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 10000) * 360);
                //placeHolder for pressure
            }

            if (m_lastVelocity != m_aircraftRigidbody.velocity)
            {

                //for Air Speed Meter

                float speed = Mathf.Abs(m_aircraftRigidbody.velocity.magnitude) * MeterToNaticalMile;
                //for bound velocity value
                speed = speed > HightestAirSpeedInNoticlPerHour ? HightestAirSpeedInNoticlPerHour : speed;
                speed = (speed / HightestAirSpeedInNoticlPerHour) * HighestAngleOfAirMeter;
                m_air_speed_rotor.rotation = Quaternion.Euler(0, 0, -speed);

                //for Vario Meter
                //for Vario Meter

                float xAngle = (m_AircraftParent.eulerAngles.x > 180 ?
                            m_AircraftParent.eulerAngles.x - 360f :
                            m_AircraftParent.eulerAngles.x);
                xAngle = Mathf.Abs(xAngle) < 1f ? 0.0f : (xAngle > 0 ? 1 : -1);

                float vario_speed = Mathf.Abs(m_aircraftRigidbody.velocity.y)
                    * MeterPerSecToFeetPerMin;
                
                vario_speed = vario_speed > HightestClimbRateInKiloFeetPerMIn ?
                    HightestClimbRateInKiloFeetPerMIn : vario_speed;
                vario_speed = (vario_speed / HightestClimbRateInKiloFeetPerMIn) *
                    HighestAngleOfVarioMeter;

                m_vario_meter_rotor.rotation = Quaternion.Euler(0, 0, xAngle * vario_speed);
            }
            m_lastVelocity = m_aircraftRigidbody.velocity;
            
        }
        private void OnAircraftRotationChanged()
        {
            //For Roll
            foreach (var item in m_AiRoll)
            {
                item.rotation = Quaternion.Euler(0, 0, 
                    m_AircraftParent.eulerAngles.z);
            }
            m_AiPitch.rotation = Quaternion.Euler(0, 0,
                    m_AircraftParent.eulerAngles.z);
            //for Pitch
            float xAngle = (m_AircraftParent.eulerAngles.x > 180 ?
                            m_AircraftParent.eulerAngles.x - 360f :
                            m_AircraftParent.eulerAngles.x);
            m_AiPitch.localPosition = new Vector2
            {
                x = 0.0f,
                y = (xAngle/180f)* m_pitchLimit
            };
            //Debug.Log(m_AircraftParent.eulerAngles.x);
            //for Yaw
            m_Yaw.rotation = Quaternion.Euler(0,0, m_AircraftParent.eulerAngles.y);
        }
        private void OnDestroy()
        {
            GameEvents.Current.onAircraftRotationChanged -= OnAircraftRotationChanged;
            GameEvents.Current.onOffsetChanged -= OnOffsetChanged;
        }
        /*
        IEnumerator UpdateFlightUI()
        {
            while (true)
            {
                //for AltMeter
                float heightFeet = m_AircraftParent.position.y * MeterToFeet;
                m_hand_100.rotation = Quaternion.Euler(0, 0, -(heightFeet / 100) * 360);
                m_hand_1000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 1000) * 360);
                m_hand_10000.rotation = Quaternion.Euler(0, 0, -(heightFeet / 10000) * 360);
                //placeHolder for pressure

                //for Air Speed Meter

                //for Vario Meter


                yield return new WaitForSeconds(m_UIUpdateInterVal);
            }
        }
        */
    }

}
