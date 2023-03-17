using System;
using System.Linq;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using TMPro;
using Assets.Scripts.SceneState;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Visuals.UiHandler
{
    public class DragAndDropHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public TextMeshProUGUI clickableText;
        public TextHandler clickableTextHandler;
        public DclEntity draggedEntity = null;
        public DropHandler dropHandlerUpper;
        public DropHandler dropHandlerCenter;
        public DropHandler dropHandlerLower;
        public RightClickHandler rightClickHandler;
        public bool isExpanded = false;
        public bool isParentExpanded = false;
        private VerticalLayoutGroup verticalLayoutGroupParent =>
            transform.parent.gameObject.GetComponent<VerticalLayoutGroup>();
        private TextHandler.TextStyle previousTextStyle;
        private bool isDragging;
        private static event Action onDragStartEvent;

        private void Awake()
        {
            onDragStartEvent += EnterDropModeOthers;
        }

        private void OnDestroy()
        {
            onDragStartEvent -= EnterDropModeOthers;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnterDragMode();
            
            onDragStartEvent?.Invoke();
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            EnterDefaultMode();
        }

        private void EnterDefaultMode()
        {
            clickableTextHandler.textStyle = previousTextStyle;
            verticalLayoutGroupParent.enabled = false;
            SetDragHandlerEnabled(true);
            verticalLayoutGroupParent.enabled = true;
            isDragging = false;
        }
        
        private void EnterDragMode()
        {
            previousTextStyle = clickableTextHandler.textStyle;
            clickableTextHandler.textStyle = TextHandler.TextStyle.Disabled;
            SetDragHandlerEnabled(false);
            isDragging = true;
        }

        private void EnterDropModeOthers()
        {
            if (isDragging)
            {
                return;
            }

            SetDragHandlerEnabled(false);
            SetDropHandlersEnabled(true);
        }

        private void SetDragHandlerEnabled(bool enabled)
        {
            clickableText.raycastTarget = enabled;
            rightClickHandler.enabled = enabled;
        }

        public void ResetHandler()
        {
            SetDropHandlersEnabled(false);
            SetDragHandlerEnabled(true);
            
            UpdateDropHandlerLowerHoverImage();
            UpdateDropHandlerUpperHoverImage();

            ResetDropHandlers();
        }

        private void SetDropHandlersEnabled(bool enabled)
        {
            dropHandlerUpper.SetEnabled(enabled);
            dropHandlerCenter.SetEnabled(enabled);
            dropHandlerLower.SetEnabled(enabled);
        }

        /// <summary>
        /// Updates the lower drop handlers current hover image
        /// </summary>
        private void UpdateDropHandlerLowerHoverImage()
        {
            if (isExpanded)
            {
                dropHandlerLower.SetCurrentHoverImageSpecial();
            }
            else
            {
                dropHandlerLower.SetCurrentHoverImageDefault();
            }
        }

        /// <summary>
        /// Updates the upper drop handlers current hover image
        /// </summary>
        private void UpdateDropHandlerUpperHoverImage()
        {
            var parent = draggedEntity?.Parent;

            if (parent == null)
            {
                dropHandlerUpper.SetCurrentHoverImageDefault();
                return;
            }

            var firstChild = parent.Children.OrderBy(e => e.hierarchyOrder).First();

            if (firstChild.Id.Equals(draggedEntity.Id))
            {
                if (isParentExpanded)
                {
                    dropHandlerUpper.SetCurrentHoverImageSpecial();
                    return;
                }
            }

            dropHandlerUpper.SetCurrentHoverImageDefault();
        }
        
        public void UpdateDropHandlers(DropActions dropActions)
        {
            dropHandlerUpper.onDrop = dropActions.dropActionUpper;
            dropHandlerCenter.onDrop = dropActions.dropActionMiddle;
            dropHandlerLower.onDrop = dropActions.dropActionLower;
        }

        private void ResetDropHandlers()
        {
            dropHandlerUpper.ResetHandler();
            dropHandlerCenter.ResetHandler();
            dropHandlerLower.ResetHandler();
        }

        //Don't remove!
        public void OnDrag(PointerEventData eventData) { }
    }
}
