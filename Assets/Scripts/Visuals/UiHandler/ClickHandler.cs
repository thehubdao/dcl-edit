using System;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [NotNull]
        public ClickStrategy clickStrategy;

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log($"Pointer Click from ClickHandler in {StaticUtilities.ListGameObjectStack(gameObject)}");

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    clickStrategy.rightClickStrategy?.onRightClick.Invoke(new ClickStrategy.EventData() {position = eventData.position, gameObject = eventData.pointerPress});
                    break;

                case PointerEventData.InputButton.Left:
                    clickStrategy.leftClickStrategy?.onLeftClick.Invoke(new ClickStrategy.EventData() {position = eventData.position, gameObject = eventData.pointerPress});
                    break;
            }
        }
    }
}
