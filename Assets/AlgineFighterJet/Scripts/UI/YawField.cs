using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Algine.Aircraft.UI
{
    public class YawField : MonoBehaviour, IDragHandler, IPointerDownHandler, 
        IPointerUpHandler
    {
        [Header("Components")]
        public RectTransform background;
        public RectTransform handle;

        float bgXpos = 0;
        void Start()
        {
            bgXpos = RectTransformUtility.WorldToScreenPoint(
                new Camera(), background.position).x;
            handle.anchoredPosition = new Vector2(0.0f, 0.0f);
            InputManager.rudderInput = 0.0f;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            float x = Mathf.Clamp((eventData.position).x - bgXpos,
                 -background.sizeDelta.x / 2, background.sizeDelta.x/2);

            InputManager.rudderInput = x / background.sizeDelta.x;
            handle.anchoredPosition = new Vector2(x,0.0f);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputManager.rudderInput = 0.0f;
            handle.anchoredPosition = new Vector2(0.0f, 0.0f);
        }
    }

}
