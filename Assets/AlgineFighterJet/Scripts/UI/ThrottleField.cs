using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Algine.Aircraft.UI
{
    public class ThrottleField : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [Header("Components")]
        public RectTransform background;
        public RectTransform handle;
        [SerializeField]
        private float minStartReq = 0.0f;
        [SerializeField]
        private float minInputReq = 0.0f;

        private float m_bgYpos = 0;
        void Start()
        {
            m_bgYpos = RectTransformUtility.WorldToScreenPoint(
                new Camera(), background.position).y;
            handle.anchoredPosition = new Vector2(0.0f,
                minStartReq * 
                background.sizeDelta.y);
            InputManager.ThrottleMove(0.0f);
            InputManager.OnSetMinPOwer += OnSetMinPower;
        }
        private void OnSetMinPower(float val)
        {
            minInputReq = val;
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            float y = Mathf.Clamp(eventData.position.y - m_bgYpos,
                minInputReq
                * background.sizeDelta.y, background.sizeDelta.y);

            InputManager.ThrottleMove( y / background.sizeDelta.y);

            handle.anchoredPosition = new Vector2(0.0f,y);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }
    }
}

