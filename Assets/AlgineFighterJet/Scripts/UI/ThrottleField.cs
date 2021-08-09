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

        public float MinimumRequired = 0.0f;

        float bgYpos = 0;
        void Start()
        {
            MinimumRequired = InputManager.throttleInputMinimum;
            bgYpos = RectTransformUtility.WorldToScreenPoint(
                new Camera(), background.position).y;
            handle.anchoredPosition = new Vector2(0.0f,
                MinimumRequired* 
                background.sizeDelta.y);
            InputManager.throttleInput = 0.0f;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            MinimumRequired = InputManager.throttleInputMinimum;

            float y = Mathf.Clamp(eventData.position.y - bgYpos,
                MinimumRequired
                * background.sizeDelta.y, background.sizeDelta.y);

            InputManager.throttleInput = y / background.sizeDelta.y;

            handle.anchoredPosition = new Vector2(0.0f,y);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }
    }
}

