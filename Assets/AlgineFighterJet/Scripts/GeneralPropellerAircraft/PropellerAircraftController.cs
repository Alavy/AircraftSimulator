using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Algine.Aircraft.UI;
using Algine.Aircraft;
using UnityEditorInternal;

namespace Algine.GeneraPropellerAircraft
{
    public class PropellerAircraftController : MonoBehaviour
    {
        #region Aircraft_Orientation
        [SerializeField]
        private Vector3 sensitivityForAicraft = new Vector3(20f, 5f, 4f);
        [SerializeField]
        private float m_timetoReturnDefaultState = .9f;
        [SerializeField]
        private float m_minimumHeightToRoll = 20f;
        [SerializeField]
        private LayerMask m_checkingHeight;

        private Vector2 _smoothMouseForAircraft = Vector2.zero;
        private float _yawValue = 0;

        private bool m_isCloseToGround = true;
        private bool m_isCloseToMaxiMumCeling = false;

        private Vector2 m_sideStickVec = Vector2.zero;
        private float m_throttleVal = 0.0f;
        private float m_rudderVal = 0.0f;
        #endregion

        #region Prop_Engine
        [SerializeField]
        private AudioSource m_audioSource;
        [SerializeField]
        private AudioClip m_engineStartSound;
        [SerializeField]
        private AudioClip m_engineRunningSound;
        [SerializeField]
        private float m_propellerSpeed = 30f;
        [SerializeField]
        private Transform[] m_prop;
        [SerializeField]
        private GameObject[] m_low_speed_propellers;
        [SerializeField]
        private GameObject[] m_high_speed_propellers;

        [SerializeField]
        private float m_startEngineTime = 3f;
        [SerializeField]
        private float m_startEngineVelocity = 15000f;
        [SerializeField]
        private float m_minimumTakeOffPower = .4f;
        [SerializeField]
        private float m_minimumPower = .2f;

        private bool m_isAircraftStoped = true;
        private bool m_isEngineStarted = false;
        private bool oneTimeForStart = true;

        private bool m_isHighSpeedProp = true;
        private bool m_isLowSpeedProp = false;
        #endregion

        private Animator m_animaor;
        private string horizontal = "horizontal";
        private string vertical = "vertical";
        private string paddle = "paddle";

        private PropellerAircraft m_aircraft;

        
        private bool m_brake = false;

        [SerializeField]
        private LayerMask m_ground;
        [SerializeField]
        [Range(1,4f)]
        private float m_checkDistance=1f;

        [SerializeField]
        [Range(.1f, 4f)]
        private float m_brakeTimeMultiplier = .94f;

        private LTDescr m_default_xAnim;
        private LTDescr m_default_zAnim;
        private bool m_returnTodefaultState = true;
        [HideInInspector]
        public double AircraftPositionOffsetX { get; private set; }
        [HideInInspector]
        public double AircraftPositionOffsetY { get; private set; }
        [HideInInspector]
        public double AircraftPositionOffsetZ { get; private set; }

        //previous States
        private Vector3 m_lastEulerAngles=Vector3.zero;
        [SerializeField]
        private double m_maximumCeilingHeight=10000;

        private void Start()
        {
            m_animaor = GetComponent<Animator>();
            m_aircraft = GetComponent<PropellerAircraft>();

            InputManager.OnRudderMove += OnRudderMove;
            InputManager.OnSideStickMove += OnSideStickMove;
            InputManager.OnThrottleMove += OnThrottleMove;

            if (m_low_speed_propellers == null ||
                m_high_speed_propellers == null ||
                m_prop == null)
            {
                Debug.LogError("propeller missing");
            }
            if(m_animaor == null)
            {
                Debug.LogError("Animator missing");
            }
            if (m_aircraft == null)
            {
                Debug.LogError("Propeller Aircraft missing");
            }
            if(m_audioSource == null)
            {
                Debug.LogError("please insert Audio source");
            }
            //default 
            m_default_xAnim = transform.LeanRotateX(0, m_timetoReturnDefaultState);
            m_default_zAnim = transform.LeanRotateZ(0, m_timetoReturnDefaultState);

            foreach (var item in m_high_speed_propellers)
            {
                item.gameObject.SetActive(false);
            }
            ///initial condition
            m_aircraft.m_dragForce = 0;
            m_aircraft.m_thrustForce = 0;
            m_aircraft.m_liftForce = 0;
            m_aircraft.m_weight = 0;

            // initialize previous States
            m_lastEulerAngles = transform.eulerAngles;
            GameEvents.Current.onOffsetChanged += OnOffsetChanged;
        }
        private void OnOffsetChanged(Vector3 deltaOffeset)
        {
            AircraftPositionOffsetX += deltaOffeset.x;
            AircraftPositionOffsetY += deltaOffeset.y;
            AircraftPositionOffsetZ += deltaOffeset.z;
        }
        public (double, double, double) getAbsolutePosition()
        {
            return (
                AircraftPositionOffsetX + transform.position.x,
                AircraftPositionOffsetY + transform.position.y,
                AircraftPositionOffsetZ + transform.position.z
               );
        }
        private void OnSideStickMove(Vector2 dir)
        {
            m_sideStickVec = dir;
        }
        private void OnRudderMove(float val)
        {
            m_rudderVal = val;
        }
        private void OnThrottleMove(float val)
        {
            m_throttleVal = val;
        }
        private void Update()
        {
            handleAnimation();
            handleAircraftPower();
            handleAircraftOrientation();
 
           
        }
        private void handleAnimation()
        {
            //passing value to the animation state Machine 
            m_animaor.SetFloat(horizontal,
                m_sideStickVec.x);
            m_animaor.SetFloat(vertical,
                m_sideStickVec.y);
            m_animaor.SetFloat(paddle,
                m_rudderVal);
        }
        private void handleAircraftPower()
        {
            float thottleInput = m_throttleVal;
            double altitude = getAbsolutePosition().Item2;

            if (altitude < m_checkDistance)
            {
                m_isCloseToGround = true;
            }
            else if (altitude >= m_maximumCeilingHeight)
            {
                m_isCloseToMaxiMumCeling = true;
            }
            else
            {
                m_isCloseToGround = false;
                m_isCloseToMaxiMumCeling = false;
            }

            if (thottleInput >= m_minimumTakeOffPower
                && !m_isEngineStarted)
            {
                InputManager.SetMinPOwer(m_minimumPower);

                StartCoroutine(startEngine());
            }

            if(!m_isAircraftStoped )
            {
                if (m_brake && m_isCloseToGround)
                {
                    m_aircraft.m_dragForce = Mathf.Lerp(m_aircraft.m_dragForce,
                        m_aircraft.m_thrustForce, Time.deltaTime * m_brakeTimeMultiplier);

                    if ((m_aircraft.m_dragForce + .4f) >=
                        m_aircraft.m_thrustForce)
                    {
                        m_aircraft.m_dragForce = 0;
                        m_aircraft.m_thrustForce = 0;
                        m_aircraft.m_liftForce = 0;
                        m_aircraft.m_weight = 0;

                        m_isAircraftStoped = true;
                        m_isEngineStarted = false;
                        oneTimeForStart = true;

                        foreach (var item in m_prop)
                        {
                            LeanTween.rotateAround(item.gameObject,
                                item.forward,
                                0,
                                m_startEngineTime);
                        }
                        //return to default orientation
                        //transform.LeanRotate(m_initOrientation, m_startEngineTime);
                        m_audioSource.Stop();

                    }

                }
                else if (m_isCloseToMaxiMumCeling)
                {
                    m_aircraft.m_thrustForce = (m_aircraft.m_Max_thrustForce * thottleInput);
                    m_aircraft.m_dragForce
                      = (m_aircraft.m_Max_dragForce * thottleInput);
                    m_aircraft.m_liftForce
                      = (m_aircraft.m_Max_liftForce * thottleInput);

                    m_aircraft.m_weight = Mathf.Lerp(m_aircraft.m_weight,
                        2 * m_aircraft.m_liftForce, Time.deltaTime * 3f);
                }
                else
                {
                    m_aircraft.m_thrustForce = (m_aircraft.m_Max_thrustForce 
                        * thottleInput);
                    m_aircraft.m_dragForce
                      = (m_aircraft.m_Max_dragForce * thottleInput);
                    m_aircraft.m_liftForce
                      = (m_aircraft.m_Max_liftForce * thottleInput);
                    m_aircraft.m_weight = m_aircraft.m_liftForce;

                }
                //engine volume
                m_audioSource.volume = Mathf.Clamp(thottleInput, .3f, 1f);

                //propeller handler
                if (thottleInput >= m_minimumTakeOffPower && m_isHighSpeedProp)
                {
                    foreach (var item in m_low_speed_propellers)
                    {
                        item.SetActive(false);
                    }
                    foreach (var item in m_high_speed_propellers)
                    {
                        item.SetActive(true);
                    }
                    m_isHighSpeedProp = false;
                    m_isLowSpeedProp = true;
                }
                else if(thottleInput < m_minimumTakeOffPower && m_isLowSpeedProp)
                {
                    foreach (var item in m_low_speed_propellers)
                    {
                        item.SetActive(true);
                    }
                    foreach (var item in m_high_speed_propellers)
                    {
                        item.SetActive(false);
                    }
                    m_isLowSpeedProp = false;
                    m_isHighSpeedProp = true;
                }
                //add torque to the propeller
                foreach (var item in m_prop)
                {
                    item.RotateAround(item.position,
                        item.forward, m_propellerSpeed);
                }

                if (oneTimeForStart)
                {
                    m_audioSource.Stop();
                    m_audioSource.clip = m_engineRunningSound;
                    m_audioSource.loop = true;
                    m_audioSource.Play();

                    oneTimeForStart = false;
                }

            }
        }
        public void brake(bool brake)
        {
            if(m_throttleVal <= m_minimumTakeOffPower)
            {
                m_brake = brake;
            }
            else
            {
                m_brake = false;
            }
           
        }
        private IEnumerator startEngine()
        {
            m_isEngineStarted = true;
            m_audioSource.Stop();
            m_audioSource.volume = m_throttleVal;
            m_audioSource.clip = m_engineStartSound;
            m_audioSource.Play();

            foreach (var item in m_prop)
            {
                LeanTween.rotateAround(item.gameObject,
                    item.forward, 
                    m_startEngineVelocity,
                    m_startEngineTime*2f);
            }
            //return to fly state
            transform.LeanRotate(Vector3.zero, 
                0.5f);

            yield return new WaitForSeconds(m_startEngineTime / 2);
            //run
            float thottleInput = m_throttleVal;

            m_aircraft.m_thrustForce
                    = (m_aircraft.m_Max_thrustForce * thottleInput);
            m_aircraft.m_dragForce
              = (m_aircraft.m_Max_dragForce * thottleInput);
            m_aircraft.m_liftForce
              = (m_aircraft.m_Max_liftForce * thottleInput);
            m_aircraft.m_weight = m_aircraft.m_liftForce;

            yield return new WaitForSeconds(
                m_startEngineTime);

            m_isAircraftStoped = false;  
            
        }
        private void handleAircraftOrientation()
        {
            if(!m_isAircraftStoped)
            {
                if (m_rudderVal != 0)
                {
                    float yawDelata = m_rudderVal
                    * sensitivityForAicraft.z 
                    * m_throttleVal;
                    _yawValue = Mathf.Lerp(yawDelata, _yawValue,
                                Time.deltaTime);

                    transform.RotateAround(transform.position,
                    transform.up, _yawValue);

                }
                //checking SideStick input are zero
                if (m_sideStickVec.x == 0
                    &&
                    m_sideStickVec.y == 0)
                {
                    _smoothMouseForAircraft = Vector2.zero;

                    if (m_returnTodefaultState)
                    {
                        m_default_xAnim = transform.LeanRotateX(0, m_timetoReturnDefaultState);
                        m_default_zAnim = transform.LeanRotateZ(0, m_timetoReturnDefaultState);

                        m_returnTodefaultState = false;
                    }

                }
                else
                {

                    m_default_xAnim.reset();
                    m_default_zAnim.reset();

                    Vector2 mouseDelta = new Vector2(
                            m_sideStickVec.x * m_throttleVal
                            * sensitivityForAicraft.x,
                            m_sideStickVec.y * m_throttleVal
                            * sensitivityForAicraft.y);

                    _smoothMouseForAircraft = Vector2.Lerp(_smoothMouseForAircraft,
                        mouseDelta, Time.deltaTime);

                    transform.RotateAround(transform.position, transform.right,
                       -_smoothMouseForAircraft.y);
                    transform.RotateAround(transform.position, transform.forward,
                       -_smoothMouseForAircraft.x);
                    m_returnTodefaultState = true;

                }
                if(m_lastEulerAngles != transform.eulerAngles)
                {
                    GameEvents.Current.AircraftRotationChanged();
                }

                // update previous States
                m_lastEulerAngles = transform.eulerAngles;
            }

        }
        private void OnDestroy()
        {
            GameEvents.Current.onOffsetChanged -= OnOffsetChanged;
        }
    }
}