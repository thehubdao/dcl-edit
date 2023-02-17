using System.Linq;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using TMPro;

namespace Visuals.UiHandler
{
    using Assets.Scripts.SceneState;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class DragAndDropHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public TextMeshProUGUI clickableText;
        public TextHandler clickableTextHandler;
        public DclEntity draggedEntity = null;
        public DropHandler dropHandlerUpper;
        public DropHandler dropHandlerCenter;
        public DropHandler dropHandlerLower;
        public RightClickHandler rightClickHandler;
        
        private VerticalLayoutGroup verticalLayoutGroupParent;
        private bool isBeingDragged = false;
        private bool init = false;
        private TextHandler.TextStyle previousTextStyle;
        public bool isExpanded = false;
        public bool isParentExpanded = false;


        private void Start()
        {
            init = true;
            verticalLayoutGroupParent = transform.parent.gameObject.GetComponent<VerticalLayoutGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            previousTextStyle = clickableTextHandler.textStyle;
            clickableTextHandler.textStyle = TextHandler.TextStyle.Disabled;
            SetDragHandlerEnabled(false);
            rightClickHandler.enabled = false;


            isBeingDragged = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            clickableTextHandler.textStyle = previousTextStyle;
            verticalLayoutGroupParent.enabled = false;

            SetDragHandlerEnabled(true);
            rightClickHandler.enabled = true;

            verticalLayoutGroupParent.enabled = true;

            isBeingDragged = false;
        }


        //Called for all Handlers that aren't dragged
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isBeingDragged && eventData.pointerDrag != null)
            {
                SetDragHandlerEnabled(false);
                rightClickHandler.enabled = false;
                SetDropHandlersEnabled(true);
                UpdateDropHandlerLowerHoverImage();
                UpdateDropHandlerUpperHoverImage();
            }
        }

        //Called for all Handlers that aren't dragged
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isBeingDragged && eventData.pointerDrag != null)
            {
                SetDragHandlerEnabled(true);
                rightClickHandler.enabled = true;
                SetDropHandlersEnabled(false);
            }
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

        private void SetDragHandlerEnabled(bool enabled)
        {
            clickableText.raycastTarget = enabled;
        }

        public void ResetHandler()
        {
            if (init)
            {
                SetDropHandlersEnabled(false);
                SetDragHandlerEnabled(true);

                rightClickHandler.enabled = true;

                dropHandlerUpper.ResetHandler();
                dropHandlerCenter.ResetHandler();
                dropHandlerLower.ResetHandler();
            }
        }

        public void UpdateDropHandlers(DropActions dropActions)
        {
            dropHandlerUpper.onDrop = dropActions.dropActionUpper;
            dropHandlerCenter.onDrop = dropActions.dropActionMiddle;
            dropHandlerLower.onDrop = dropActions.dropActionLower;
        }
        
        //Don't remove!
        public void OnDrag(PointerEventData eventData) { }
    }
}
