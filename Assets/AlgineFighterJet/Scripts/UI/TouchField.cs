using UnityEngine;
using UnityEngine.EventSystems;

namespace Algine.Aircraft.UI
{
    public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
    {
        [SerializeField]
        private float moveRange=50f;
        private Vector2 PointerOld;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            PointerOld = eventData.position;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //InputManager.touchPanelLook = Vector2.zero;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            InputManager.TouchLook(Vector2.ClampMagnitude( eventData.position 
                - PointerOld,moveRange)/moveRange);

            PointerOld = eventData.position;
        }
    }
}