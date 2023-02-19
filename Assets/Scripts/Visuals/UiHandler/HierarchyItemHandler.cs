using System;
using Assets.Scripts.Visuals.UiBuilder;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Visuals.UiHandler;

namespace Assets.Scripts.Visuals.UiHandler
{
    public class HierarchyItemHandler : MonoBehaviour
    {
        [SerializeField]
        public TextHandler text;

        [SerializeField]
        private RectTransform arrow;

        [SerializeField]
        private RectTransform arrowContainer;

        [SerializeField]
        public RectTransform indent;

        [SerializeField]
        public RightClickHandler rightClickHandler;
        
        [SerializeField]
        public DragAndDropHandler dragAndDropHandler;

        public bool primarySelection;

        public struct UiHierarchyItemActions
        {
            [CanBeNull]
            public Action onArrowClick;

            [CanBeNull]
            public Action onNameClick;
        }

        public UiHierarchyItemActions actions
        {
            set
            {
                arrow.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                text.GetComponent<Button>().onClick.RemoveAllListeners();

                arrow.GetComponent<Toggle>().onValueChanged.AddListener(_ => value.onArrowClick?.Invoke());
                text.GetComponent<Button>().onClick.AddListener(() => value.onNameClick?.Invoke());
            }
        }

        public bool isExpanded
        {
            set
            {
                if (value)
                {
                    arrowContainer.rotation = Quaternion.Euler(0, 0, 0);
                    arrowContainer.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    arrowContainer.rotation = Quaternion.Euler(0, 0, 90);
                    arrowContainer.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        public bool showArrow
        {
            set => arrowContainer.gameObject.SetActive(value);
        }

        public void UpdateHandlers(HierarchyItemAtom.Data newHierarchyItemData)
        {
            actions = newHierarchyItemData.actions;
            rightClickHandler.onRightClick = newHierarchyItemData.rightClickAction;
            dragAndDropHandler.UpdateDropHandlers(newHierarchyItemData.dropActions);
            dragAndDropHandler.draggedEntity = newHierarchyItemData.draggedEntity;
            dragAndDropHandler.isExpanded = newHierarchyItemData.isExpanded;
            dragAndDropHandler.isParentExpanded = newHierarchyItemData.isParentExpanded;
            
            primarySelection = newHierarchyItemData.isPrimarySelected;
            
            text.text = newHierarchyItemData.name;
            text.textStyle = newHierarchyItemData.style;
            text.TextComponent.enabled = true;
            
            indent.offsetMin = new Vector2(20 * newHierarchyItemData.level, 0);

            if (newHierarchyItemData.hasChildren)
            {
                showArrow = true;
                isExpanded = newHierarchyItemData.isExpanded;
            }
            else
            {
                showArrow = false;
            }
            
            dragAndDropHandler.ResetHandler();
        }
        
    }
}
