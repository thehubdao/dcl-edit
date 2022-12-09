using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class RightClickHandler : MonoBehaviour, IPointerDownHandler
    {
        public Action<Vector3> onRightClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                onRightClick?.Invoke(new Vector3(eventData.position.x, eventData.position.y));
            }
        }
    }
}
