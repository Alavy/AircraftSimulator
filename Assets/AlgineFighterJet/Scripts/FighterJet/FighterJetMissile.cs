using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.FighterJet
{
    public class FighterJetMissile : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_AfterBurner;
        [SerializeField]
        private float launchDelay=1.3f;
        [SerializeField]
        private float m_thurstForce = 3000f;
        [SerializeField]
        private float m_weight = 500f;
        [SerializeField]
        private float m_Mass = 30f;
        [SerializeField]
        private float RotTurn = 50f;

        [SerializeField]
        private GameObject m_Explosion;
        private GameObject tempExplosion;

        private Vector3 m_TargetPos;
        private Rigidbody m_Rigidbody;
       

        private bool m_isOnPower = false;
        private bool m_initState = false;
        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_AfterBurner.SetActive(false);

            tempExplosion = Instantiate(m_Explosion);
            tempExplosion.SetActive(false);
        }
        public void StartMissile(Vector3 target_point)
        {
            m_TargetPos = target_point;
            StartCoroutine(StartDelay());
        }
        IEnumerator StartDelay()
        {
            m_initState = true;
            yield return new WaitForSeconds(launchDelay);
            m_isOnPower = true;
            m_initState = false;
            m_AfterBurner.SetActive(true);
        }
        private void FixedUpdate()
        {
            if (m_isOnPower)
            {
                m_Rigidbody.velocity = (m_thurstForce * transform.forward) / m_Mass;
                Quaternion rot = Quaternion.LookRotation(m_TargetPos
                    - transform.position);
                m_Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, 
                    rot, Time.deltaTime * RotTurn));
            }
            if(m_initState)
            {
                m_Rigidbody.velocity = (m_weight * - transform.up) / m_Mass;
            }
            
        }
        private void OnCollisionEnter(Collision collision)
        {
            ContactPoint contactPoint = collision.GetContact(0);
            tempExplosion.transform.position = contactPoint.point
                +contactPoint.normal;
            tempExplosion.SetActive(true);
            Destroy(gameObject);
        }
    }
}