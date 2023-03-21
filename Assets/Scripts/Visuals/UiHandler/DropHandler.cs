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

        private void Awake()
        {
            currentHoverImage = hoverImageDefault;
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
            }
            
            ShowCurrentImage();
        }

        //TODO make more efficient by preventing all scroll rects from being draggable
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!rayCastTarget || eventData.pointerDrag == null ||!CheckIsDraggable(eventData))
            {
                return;
            }

            HideImages();
        }

        private void ShowCurrentImage()
        {
            currentHoverImage.enabled = true;
        }

        private void HideImages()
        {
            currentHoverImage.enabled = false;
            hoverImageDefault.enabled = false;
            hoverImageSpecial.enabled = false;
        }
        
        public void SetEnabled(bool enabled)
        {
            clickableImage.raycastTarget = enabled;
            rayCastTarget = enabled;
        }

        public void ResetHandler()
        {
            HideImages();
            SetEnabled(false);
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
