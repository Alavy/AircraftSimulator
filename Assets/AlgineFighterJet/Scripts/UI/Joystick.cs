using UnityEngine;
using UnityEngine.EventSystems;

namespace Algine.Aircraft.UI
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Options")]
        [Range(0f, 2f)] public float handleLimit = 1f;

        public static Vector2 inputVector = Vector2.zero;

        [Header("Components")]
        public RectTransform background;
        public RectTransform handle;

        Vector2 joystickPosition = Vector2.zero;

        void Start()
        {
            joystickPosition = RectTransformUtility.WorldToScreenPoint(
                new Camera(),background.position);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - joystickPosition;
            inputVector = (direction.magnitude > background.sizeDelta.x / 2f) 
                ? direction.normalized : direction / (background.sizeDelta.x / 2f);
            handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
            InputManager.joystickInputVector = inputVector;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);

            //LeanTween.scale(gameObject, new Vector2 { x = 1.3f, y = 1.3f}, .05f);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            InputManager.joystickInputVector = inputVector;
            handle.anchoredPosition = Vector2.zero;

            //LeanTween.scale(gameObject, new Vector2 { x = 1f, y = 1f }, .05f);
        }
    }
}
