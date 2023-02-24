namespace Visuals.UiHandler
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class DropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<GameObject> onDrop;
        public Image clickableImage;
        public Image hoverImageDefault;
        public Image hoverImageSpecial;
        private Image currentHoverImage;
        [HideInInspector]
        public bool rayCastTarget = false;
        private bool init = false;

        private void Start()
        {
            currentHoverImage = hoverImageDefault;
            init = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var draggedGameObject = eventData.pointerDrag;

            if (!rayCastTarget || draggedGameObject == null || !CheckIsDraggable(eventData))
            {
                return;
            }

            onDrop?.Invoke(draggedGameObject);
        }

        //TODO make more efficient by preventing all scroll rects from being draggable
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!rayCastTarget || eventData.pointerDrag == null || !CheckIsDraggable(eventData))
            {
                return;
            };
            
            ChangeImageAlpha(0.1f);
        }

        //TODO make more efficient by preventing all scroll rects from being draggable
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!rayCastTarget || eventData.pointerDrag == null ||!CheckIsDraggable(eventData))
            {
                return;
            }

            ChangeImageAlpha(0);
        }

        private void ChangeImageAlpha(float alpha)
        {
            var value = Mathf.Clamp(alpha, 0, 1);

            var imageColor = currentHoverImage.color;
            imageColor.a = value;
            currentHoverImage.color = imageColor;
        }
        
        public void SetEnabled(bool enabled)
        {
            clickableImage.raycastTarget = enabled;
            this.rayCastTarget = enabled;
        }

        public void ResetHandler()
        {
            if (!init)
            {
                return;
            }
            
            SetEnabled(false);
            ChangeImageAlpha(0);
        }

        private static bool CheckIsDraggable(PointerEventData eventData)
        {
            return eventData.pointerDrag.TryGetComponent(out DragAndDropHandler dragAndDropHandler);
        }

        public void SetCurrentHoverImageDefault()
        {
            currentHoverImage = hoverImageDefault;
        }
        
        public void SetCurrentHoverImageSpecial()
        {
            currentHoverImage = hoverImageSpecial;
        }
    }
}
