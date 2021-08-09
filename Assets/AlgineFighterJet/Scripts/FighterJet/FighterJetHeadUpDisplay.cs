using Algine.Aircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Algine.FighterJet
{
    public class FighterJetHeadUpDisplay : MonoBehaviour
    {
        [SerializeField]
        private RawImage Compass_Bar;
        [SerializeField]
        private FighterJetController m_controller;
        [SerializeField]
        private Image m_target;

        void Start()
        {
            Compass_Bar.uvRect = new Rect(
                (m_controller.transform.eulerAngles.y / 360f) - .5f,
                0f, 1f, 1f);
            GameEvents.Current.onAircraftRotationChanged += UpdateCompassBar;
            GameEvents.Current.onTargetUnLock += OnTargetUnLock;
            GameEvents.Current.onTargetLock += OnTargetLock;
        }
        private void OnTargetLock(Vector3 targetPos)
        {
            m_target.color = Color.red;
        }
        private void OnTargetUnLock()
        {
            m_target.color = Color.black;
        }
        private void UpdateCompassBar()
        {
            // set compass bar texture coordinates
            Compass_Bar.uvRect = new Rect(
                (m_controller.transform.eulerAngles.y / 360f) - .5f,
                0f, 1f, 1f);
        }
        private void OnDestroy()
        {
            GameEvents.Current.onAircraftRotationChanged -= UpdateCompassBar;
            GameEvents.Current.onTargetLock -= OnTargetLock;
        }
    }
}