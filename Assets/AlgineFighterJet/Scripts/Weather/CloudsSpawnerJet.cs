using Algine.Aircraft.UI;
using Algine.FighterJet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.Aircraft
{
    public class CloudsSpawnerJet : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_cloudPrefab;
        [SerializeField]
        private Transform m_cloudsSpawnPoints;

        private GameObject tempCloudPrefab;
        private float m_cloudSpawnInterval = .5f;
        [SerializeField]
        private float m_cloudHeight = 1000f;
        [SerializeField]
        private float m_cloudFastIntervalThresold = .6f;
        [SerializeField]
        private float m_cloudFastInterval = .2f;
        [SerializeField]
        private float m_cloudSlowInterval = 1f;
        private float m_throttleVal = 0.0f;

        private FighterJetController m_controller;

        private void Start()
        {
            tempCloudPrefab = Instantiate(m_cloudPrefab);
            tempCloudPrefab.SetActive(false);
            InputManager.OnThrottleMove += OnThrottleMove;

            m_controller = GetComponent<FighterJetController>();

            StartCoroutine(SpawnCloud());
            GameEvents.Current.onAfterOriginChanged += OnAfterOriginChanged;
        }
        private void OnThrottleMove(float val)
        {
            m_throttleVal = val;
        }
        private void OnAfterOriginChanged(Vector3 playerPos)
        {
            tempCloudPrefab.transform.position = m_cloudsSpawnPoints.position;
            double altitude = m_controller.getAbsolutePosition().Item2;
            if (altitude >= m_cloudHeight)
            {
                tempCloudPrefab.SetActive(true);
            }
            else
            {
                tempCloudPrefab.SetActive(false);
            }
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

                    if (m_throttleVal >= m_cloudFastIntervalThresold)
                    {
                        m_cloudSpawnInterval = m_cloudFastInterval;
                    }
                    else
                    {
                        m_cloudSpawnInterval = m_cloudSlowInterval;
                    }
                }
                else
                {
                    tempCloudPrefab.SetActive(false);
                }
                yield return new WaitForSeconds(m_cloudSpawnInterval);
            }
        }
        private void OnDestroy()
        {
            GameEvents.Current.onAfterOriginChanged -= OnAfterOriginChanged;
        }
    }
}