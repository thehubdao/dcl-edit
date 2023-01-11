using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class MouseHoverSceneButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {

        public bool IsMouseOverGizmoModeMenu { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.gameObject.GetComponent<Button>().onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Mouse OVER button");
            IsMouseOverGizmoModeMenu = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Mouse EXIT button");
            IsMouseOverGizmoModeMenu = false;
        }
    }
}
