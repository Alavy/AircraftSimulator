using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.FighterJet
{
    public class FighterJet : MonoBehaviour
    {
        #region JetEngineProperties
        [SerializeField]
        public float m_Max_thrustForce = 800f;
        [SerializeField]
        public float m_Max_dragForce = 300f;
        [SerializeField]
        public float m_Max_liftForce = 200f;


        [HideInInspector]
        public float m_thrustForce = 2000f;
        [HideInInspector]
        public float m_dragForce = 100f;
        [HideInInspector]
        public float m_liftForce = 400f;
        [HideInInspector]
        public float m_weight = 400f;

        private Vector3 m_thrustVectorDirection;
        private Vector3 m_dragForceDirection;
        private Vector3 m_liftForceDirection;
        private Vector3 m_weightForceDirection;

        #endregion

        #region PhysicsCalculation
        private Rigidbody m_rigidbody;
        #endregion

        void Start()
        {
            m_thrustVectorDirection = transform.forward;
            m_dragForceDirection = -transform.forward;
            m_liftForceDirection = transform.up;
            m_weightForceDirection = -Vector3.up;
            m_rigidbody = GetComponent<Rigidbody>();
          
        }
        private void FixedUpdate()
        {
            m_thrustVectorDirection = transform.forward;
            m_dragForceDirection = -transform.forward;
            m_liftForceDirection = transform.up;
            m_weightForceDirection = -Vector3.up;

            Vector3 combinedFoce = m_thrustForce * m_thrustVectorDirection +
                                   m_dragForce * m_dragForceDirection +
                                   m_liftForce * m_liftForceDirection +
                                   m_weight * m_weightForceDirection ;

            if(m_weight == 0)
            {
                m_rigidbody.velocity = Vector3.zero;
                //transform.position = m_initialpos.position;
            }
            else
            {
                m_rigidbody.velocity = (combinedFoce / (m_rigidbody.mass / 10));
            }

        }
    }
}

