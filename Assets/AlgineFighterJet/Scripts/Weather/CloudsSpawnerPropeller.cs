using Algine.GeneraPropellerAircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.Aircraft
{
    public class CloudsSpawnerPropeller : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_cloudPrefab;
        [SerializeField]
        private Transform m_cloudsSpawnPoints;

        private GameObject tempCloudPrefab;

        [SerializeField]
        private float m_cloudSpawnInterval = .5f;
        [SerializeField]
        private float m_cloudHeight = 1000f;

        private PropellerAircraftController m_controller;

        private void Start()
        {
            tempCloudPrefab =  Instantiate(m_cloudPrefab);
            tempCloudPrefab.SetActive(false);

            m_controller = GetComponent<PropellerAircraftController>();

            StartCoroutine(SpawnCloud());
        }
        IEnumerator SpawnCloud()
        {
            while (true)
            {
                double altitude = m_controller.getAbsolutePosition().Item2;
                if (altitude >= m_cloudHeight)
                {
                    tempCloudPrefab.transform.position = m_cloudsSpawnPoints.position;
                    tempCloudPrefab.SetActive(true);
                }
                else
                {
                    tempCloudPrefab.SetActive(false);
                }
                yield return new WaitForSeconds(m_cloudSpawnInterval);
            }
            
        }
    }
}