using System;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [CanBeNull]
        public RightClickStrategy rightClickStrategy;

        [CanBeNull]
        public LeftClickStrategy leftClickStrategy;

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log($"Pointer Click from ClickHandler in {StaticUtilities.ListGameObjectStack(gameObject)}");

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
