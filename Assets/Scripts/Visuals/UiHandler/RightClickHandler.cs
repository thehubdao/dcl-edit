using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class RightClickHandler : MonoBehaviour, IPointerDownHandler
    {
        public Action<Vector3> rightClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                rightClick(new Vector3(eventData.position.x, eventData.position.y));
            }

            int i;
        }
    }
}
