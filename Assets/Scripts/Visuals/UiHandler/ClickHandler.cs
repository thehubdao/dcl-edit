using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class ClickHandler : MonoBehaviour, IPointerDownHandler
    {
        [CanBeNull]
        public RightClickStrategy rightClickStrategy;

        [CanBeNull]
        public LeftClickStrategy leftClickStrategy;

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    rightClickStrategy?.onRightClick.Invoke(new ClickStrategy.EventData() {position = eventData.position});
                    break;

                case PointerEventData.InputButton.Left:
                    leftClickStrategy?.onLeftClick.Invoke(new ClickStrategy.EventData() {position = eventData.position});
                    break;
            }
        }
    }
}
