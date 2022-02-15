using UnityEngine;
using UnityEngine.EventSystems;

namespace Algine.Aircraft.UI
{
    public class SideStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Options")]
        [Range(0f, 2f)] public float handleLimit = 1f;

        [Header("Components")]
        public RectTransform background;
        public RectTransform handle;

        private Vector2 m_startPos = Vector2.zero;

        void Start()
        {
            m_startPos = RectTransformUtility.WorldToScreenPoint(
                new Camera(),background.position);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - m_startPos;
            direction = (direction.magnitude > background.sizeDelta.x / 2f) 
                ? direction.normalized : direction / (background.sizeDelta.x / 2f);
            handle.anchoredPosition = (direction * background.sizeDelta.x / 2f) * handleLimit;
            InputManager.SideStickMove(direction);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            InputManager.SideStickMove(Vector2.zero);
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
