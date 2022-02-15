using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Algine.Aircraft.UI;
using Algine.Aircraft;

namespace Algine.FighterJet
{
    public class FighterJetController : MonoBehaviour
    {
        #region Aircraft_Orientation
        [SerializeField]
        private Vector3 sensitivityForAicraft = new Vector3(1f, 1f, 1f);
        [SerializeField]
        private float m_timetoReturnDefaultState = .9f;
        [SerializeField]
        private float m_maximumCeilingHeight = 10000f;

        [SerializeField]
        private GameObject m_throttle_stick;
        [SerializeField]
        private GameObject m_joy_stick;
        private float _yawValue = 0;
        private Vector2 _smoothMouseForAircraft = Vector2.zero;

        private bool m_isCloseToGround=true;
        private bool m_isCloseToMaxiMumCeling = false;

        private Vector2 m_sideStickVec = Vector2.zero;
        private float m_throttleVal = 0.0f;
        private float m_rudderVal = 0.0f;

        #endregion

        #region JetEngine
        [SerializeField]
        private AudioSource m_audioSource;
        [SerializeField]
        private AudioClip m_engineStartSound;
        [SerializeField]
        private AudioClip m_engineRunningSound;
        [SerializeField]
        private ParticleSystem m_engine_1;
        [SerializeField]
        private ParticleSystem m_engine_2;
        [SerializeField]
        private float m_EngineSpeed = 32f;
        [SerializeField]
        private float m_EngineFlowLifeTime = .3f;

        [SerializeField]
        private float m_startEngineTime = 3f;
        [SerializeField]
        private float m_minimumTakeOffPower = .3f;
        [SerializeField]
        private float m_minimumPower = .2f;
        private bool m_isAircraftStoped = true;
        private bool m_isEngineStarted = false;
        private bool oneTimeForStart = true;
        #endregion

        #region WeaponBay
        [SerializeField]
        private FighterJetMissile[] m_Missiles;
        private int m_missileIndex = 0;
        private bool isMissleFinished = false;

        [SerializeField]
        private bool m_isTargetLock = false;
        [SerializeField]
        private Vector3 m_HitPoint = Vector3.zero;
        [SerializeField]
        private Transform m_MissileRaycas;
        [SerializeField]
        private float m_missiledistance = 1000f;
        [SerializeField]
        private LayerMask m_targetMask;
        [SerializeField]
        private float m_sphereRadius = 1f;
        #endregion

        #region Others

        private Animator m_animaor;
        private string horizontal = "horizontal";
        private string vertical = "vertical";
        private string paddle = "paddle";

        private FighterJet m_aircraft;
     

        private bool m_brake = false;

        [SerializeField]
        [Range(1, 4f)]
        private float m_checkDistance = 2f;
        [SerializeField]
        [Range(.1f,4f)]
        private float m_brakeTimeMultiplier = .94f;
       

        private bool m_landingState = false;
        private bool m_weaponBayState = false;

        private bool m_returnTodefaultState = true;

        private LTDescr m_default_xAnim;
        private LTDescr m_default_zAnim;

        private ParticleSystem.MainModule m_engine_1_burst;
       
        private ParticleSystem.MainModule m_engine_2_burst;
        
        //previous States
        private Vector3 m_lastEulerAngles = Vector3.zero;

        [HideInInspector]
        public double AircraftPositionOffsetX { get; private set; }
        [HideInInspector]
        public double AircraftPositionOffsetY { get; private set; }
        [HideInInspector]
        public double AircraftPositionOffsetZ { get; private set; }
        #endregion

        void Start()
        {
            m_animaor = GetComponent<Animator>();
            m_aircraft = GetComponent<FighterJet>();

            InputManager.OnRudderMove += OnRudderMove;
            InputManager.OnSideStickMove += OnSideStickMove;
            InputManager.OnThrottleMove += OnThrottleMove;

            if(m_engine_1 ==null || m_engine_2 == null)
            {
                Debug.LogError("Engine has no particle syatem man");
            }
            if (m_animaor == null)
            {
                Debug.LogError("Animator missing");
            }
            if (m_aircraft == null)
            {
                Debug.LogError("Propeller Aircraft missing");
            }

            ///initialize aircraft 
            m_aircraft.m_dragForce = 0;
            m_aircraft.m_thrustForce = 0;
            m_aircraft.m_liftForce = 0;
            m_aircraft.m_weight = 0;

            // Engine 1
            m_engine_1_burst = m_engine_1.main;
            
            // Engine 2
            m_engine_2_burst = m_engine_2.main;
            
            // initialize previous States
            m_lastEulerAngles = transform.eulerAngles;

            //default 
            m_default_xAnim = transform.LeanRotateX(0, m_timetoReturnDefaultState);
            m_default_zAnim = transform.LeanRotateZ(0, m_timetoReturnDefaultState);

            // handling missles first condition
            foreach (var item in m_Missiles)
            {
                item.GetComponent<Rigidbody>().isKinematic = true;
            }

            GameEvents.Current.onOffsetChanged += OnOffsetChanged;
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
        public void StarMissile()
        {
            
            if(m_isTargetLock && !isMissleFinished && !m_isCloseToGround)
            {
                m_Missiles[m_missileIndex].
                    GetComponent<Rigidbody>().isKinematic=false;
                m_Missiles[m_missileIndex].StartMissile(m_HitPoint);
                m_missileIndex++;
                if(m_missileIndex== m_Missiles.Length)
                {
                    isMissleFinished = true;
                }
                
            }
           
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
        private void Update()
        {
            handleAnimation();
            handleAircraftPower();
            handleAircraftOrientation();
            handleMissile();
        }
        private void handleMissile()
        {
            RaycastHit hit;
            if (Physics.SphereCast(m_MissileRaycas.position,
                m_sphereRadius,
                m_MissileRaycas.forward, 
                out hit, m_missiledistance, m_targetMask))
            {
                m_isTargetLock = true;
                m_HitPoint = hit.point;

                GameEvents.Current.TargetLock(m_HitPoint);
            }
            else
            {
                m_isTargetLock = false;
                m_HitPoint = Vector3.zero;
                GameEvents.Current.TargetUnLock();
            }
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

            //handle Animation through code
            if (m_throttleVal > 0)
            {
                m_throttle_stick.transform.localRotation = Quaternion.Euler(
                    m_throttleVal *
                -50, 0, 0);
            }
            m_joy_stick.transform.localRotation = Quaternion.Euler(
                    m_sideStickVec.y * -20, 0,
                m_sideStickVec.x * 20);
            
        }
        private void handleAircraftPower()
        {
            float thottleInput = m_throttleVal;
            double altitude = getAbsolutePosition().Item2;

            if ( altitude < m_checkDistance)
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
                //start engine
                StartCoroutine(startEngine());
            }

            if (!m_isAircraftStoped)
            {
                if (m_brake && m_isCloseToGround)
                {
                    m_aircraft.m_dragForce = Mathf.Lerp(m_aircraft.m_dragForce,
                        m_aircraft.m_thrustForce, Time.deltaTime *
                        m_brakeTimeMultiplier);

                    if ((m_aircraft.m_dragForce + .4f) >=
                        m_aircraft.m_thrustForce)
                    {
                        m_aircraft.m_dragForce = 0;
                        m_aircraft.m_thrustForce = 0;
                        m_aircraft.m_liftForce = 0;
                        m_aircraft.m_weight = 0;

                        //reset states
                        m_isAircraftStoped = true;
                        m_isEngineStarted = false;
                        oneTimeForStart = true;

                        //Engine Stop
                        m_engine_1.Stop();
                        m_engine_2.Stop();

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
                    m_aircraft.m_thrustForce = (m_aircraft.m_Max_thrustForce * thottleInput);
                    m_aircraft.m_dragForce
                      = (m_aircraft.m_Max_dragForce * thottleInput);
                    m_aircraft.m_liftForce
                      = (m_aircraft.m_Max_liftForce * thottleInput);
                    m_aircraft.m_weight = m_aircraft.m_liftForce;
                }
               
                m_audioSource.volume = Mathf.Clamp(thottleInput,.3f,1);

                //Engine 1
                m_engine_1_burst.startSpeed = Mathf.Clamp(m_throttleVal, .5f, 1)
                    * m_EngineSpeed;
                //Engine 2
                m_engine_2_burst.startSpeed = Mathf.Clamp(m_throttleVal, .5f, 1)
                    * m_EngineSpeed;


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
            if (m_throttleVal <= m_minimumTakeOffPower)
            {
                m_brake = brake;
            }
            else
            {
                m_brake = false;
            }

        }
        public void LandGrear()
        {
            if (!m_isCloseToGround)
            {
                m_landingState = !m_landingState;

                if (m_landingState)
                {
                    m_animaor.SetFloat("land_dir", 1);
                }
                else
                {
                    m_animaor.SetFloat("land_dir", -1);
                }
            }
        }
        public void OpenWeaponBay()
        {
            if (!m_isCloseToGround)
            {
                m_weaponBayState = !m_weaponBayState;

                if (m_weaponBayState)
                {
                    m_animaor.SetFloat("bay_dir", 1);
                }
                else
                {
                    m_animaor.SetFloat("bay_dir", -1);
                }
            }

        }
        private IEnumerator startEngine()
        {
            m_isEngineStarted = true;
            m_audioSource.Stop();
            m_audioSource.volume = 0.4f;
            m_audioSource.clip = m_engineStartSound;
            m_audioSource.loop = true;
            m_audioSource.Play();

            //Engine 1
            m_engine_1_burst.startSpeed = Mathf.Clamp(m_throttleVal, .5f, 1)
                * m_EngineSpeed;


            //Engine 2
            m_engine_2_burst.startSpeed = Mathf.Clamp(m_throttleVal, .5f, 1)
                * m_EngineSpeed;

            //Engine Start
            m_engine_1.Play();
            m_engine_2.Play();

            yield return new WaitForSeconds(m_startEngineTime/2);

            //run
            float thottleInput = m_throttleVal;
            m_aircraft.m_thrustForce
                    = (m_aircraft.m_Max_thrustForce * thottleInput);
            m_aircraft.m_dragForce
              = (m_aircraft.m_Max_dragForce * thottleInput);
            m_aircraft.m_liftForce
              = (m_aircraft.m_Max_liftForce * thottleInput);
            m_aircraft.m_weight = m_aircraft.m_liftForce;

            //return to fly state

            transform.LeanRotate(Vector3.zero,
                m_startEngineTime);

            yield return new WaitForSeconds(
                m_startEngineTime);

            m_isAircraftStoped = false;

        }
        private void handleAircraftOrientation()
        {
            if (!m_isAircraftStoped)
            {
                if (m_rudderVal != 0)
                {
                    float yawDelata = m_rudderVal
                    * sensitivityForAicraft.z 
                    * m_throttleVal;
                    //Debug.Log(InputManager.rudderInput);
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
                    transform.RotateAround(transform.position,
                        transform.forward, -_smoothMouseForAircraft.x);

                    m_returnTodefaultState = true;
                }
                if (m_lastEulerAngles != transform.eulerAngles)
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