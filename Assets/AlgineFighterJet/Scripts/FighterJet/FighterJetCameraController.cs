using Algine.Aircraft.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algine.FighterJet
{
    public class FighterJetCameraController : MonoBehaviour
    {
        #region Camera
        [SerializeField]
        private Vector2 sensitivityFoCamera = new Vector2(.01f, .01f);
        [SerializeField]
        [Range(7, 20)]
        private float cameraRotBias = 15f;
        [SerializeField]
        private Transform m_camera_holder_inside;
        [SerializeField]
        private Transform m_camera_holder_outside;
        private Vector2 _smoothMouseForCamera = Vector2.zero;
        private Vector2 _mouseAbsoluteForCamera;
        #endregion

        private Vector3 m_localPos;
        private bool m_isCamera_outside = true;

        void Start()
        {
            if (m_camera_holder_inside == null || m_camera_holder_outside == null)
            {
                Debug.LogError("camera missing");
            }
            m_localPos = transform.localPosition;
        }
        private void Update()
        {
            handleCameraOrientation();
        }
        private void handleCameraOrientation()
        {
            if (!m_isCamera_outside)
            {
                Vector2 mouseDeltaForCamera = new Vector2(
                    InputManager.touchPanelLook.x
                    * sensitivityFoCamera.x,
                    InputManager.touchPanelLook.y
                    * sensitivityFoCamera.y);

                _smoothMouseForCamera = Vector3.Lerp(_smoothMouseForCamera,
                    mouseDeltaForCamera, Time.deltaTime *
                    cameraRotBias);

                _mouseAbsoluteForCamera += _smoothMouseForCamera;

                _mouseAbsoluteForCamera.x %= 360;
                _mouseAbsoluteForCamera.y %= 360;

                transform.position = m_camera_holder_inside.position;

                m_camera_holder_inside.localRotation =
                Quaternion.Euler(-_mouseAbsoluteForCamera.y,
                _mouseAbsoluteForCamera.x, 0.0f);
            }
            else
            {
                Vector2 mouseDeltaForCamera = new Vector2(
                   InputManager.touchPanelLook.x
                   * sensitivityFoCamera.x,
                   InputManager.touchPanelLook.y
                   * sensitivityFoCamera.y);

                _smoothMouseForCamera = Vector3.Lerp(_smoothMouseForCamera,
                    mouseDeltaForCamera, Time.deltaTime *
                    cameraRotBias);

                _mouseAbsoluteForCamera += _smoothMouseForCamera;

                _mouseAbsoluteForCamera.x %= 360;
                _mouseAbsoluteForCamera.y %= 360;
                 

                m_camera_holder_outside.localRotation =
                Quaternion.Euler(-_mouseAbsoluteForCamera.y < -18f ? -18f :
                -_mouseAbsoluteForCamera.y,
                _mouseAbsoluteForCamera.x, 0.0f);
            }
        }

        public void ChangeCameraPosition()
        {
            m_isCamera_outside = !m_isCamera_outside;
            
            if (m_isCamera_outside)
            {
                transform.SetParent(m_camera_holder_outside);
                transform.localPosition = m_localPos;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.SetParent(m_camera_holder_inside);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }

        }
    }
}