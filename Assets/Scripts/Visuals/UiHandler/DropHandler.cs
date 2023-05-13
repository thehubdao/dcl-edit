using System;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Visuals.UiHandler
{
    public class DropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IDropZoneHandler
    {
        // Dependencies
        private DragAndDropState dragAndDropState;


        [Inject]
        private void Construct(DragAndDropState dragAndDropState)
        {
            this.dragAndDropState = dragAndDropState;
        }


        [CanBeNull]
        private DropStrategy dropStrategyInternal;

        [CanBeNull]
        public DropStrategy dropStrategy
        {
            get => dropStrategyInternal;
            set
            {
                var dropCategories = new List<DragAndDropState.DropZoneCategory>();
                if (value?.dropEntityStrategy != null)
                {
                    dropCategories.Add(DragAndDropState.DropZoneCategory.Entity);
                }

                if (value?.dropModelAssetStrategy != null)
                {
                    dropCategories.Add(DragAndDropState.DropZoneCategory.ModelAsset);
                }

                dragAndDropState.RegisterHandler(this, dropCategories);
                dropStrategyInternal = value;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            //Debug.Log($"Dropped on {StaticUtilities.ListGameObjectStack(gameObject)}");

            if (eventData.pointerDrag == null) return;
            if (dropStrategy == null) return;

            if (eventData.pointerDrag.TryGetComponent(out DragHandler dragHandler))
            {
                dropStrategy.OnDropped(dragHandler.dragStrategy);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
