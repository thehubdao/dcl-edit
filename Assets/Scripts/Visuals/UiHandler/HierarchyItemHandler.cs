using System;
using Assets.Scripts.Visuals.UiBuilder;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Visuals.UiHandler;
using Zenject;

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
        private ClickHandler textClickHandler;

        [SerializeField]
        private ClickHandler arrowClickHandler;

        [SerializeField]
        public DragHandler dragHandler;

        [SerializeField]
        private DropHandler upperDropHandler;

        [SerializeField]
        private DropHandler middleDropHandler;

        [SerializeField]
        private DropHandler lowerDropHandler;

        public bool primarySelection;


        //public UiHierarchyItemActions actions
        //{
        //    set
        //    {
        //        arrow.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        //        text.GetComponent<Button>().onClick.RemoveAllListeners();
        //
        //        arrow.GetComponent<Toggle>().onValueChanged.AddListener(_ => value.onArrowClick?.Invoke());
        //        text.GetComponent<Button>().onClick.AddListener(() => value.onNameClick?.Invoke());
        //    }
        //}

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
            gameObject.name = $"Hierarchy item: {newHierarchyItemData.name.value()}"; // only for debug purposes
            text.SetTextValueStrategy(newHierarchyItemData.name);
            text.textStyle = newHierarchyItemData.style;


            textClickHandler.clickStrategy = newHierarchyItemData.clickTextStrategy;
            arrowClickHandler.clickStrategy = newHierarchyItemData.clickArrowStrategy;


            primarySelection = newHierarchyItemData.isPrimarySelected;

            text.TextComponent.enabled = true;


            indent.offsetMin = new Vector2(20 * newHierarchyItemData.level, 0);

            dragHandler.dragStrategy = newHierarchyItemData.dragStrategy;

            upperDropHandler.dropStrategy = newHierarchyItemData.dropStrategyUpper;
            middleDropHandler.dropStrategy = newHierarchyItemData.dropStrategyMiddle;
            lowerDropHandler.dropStrategy = newHierarchyItemData.dropStrategyLower;

            if (newHierarchyItemData.hasChildren)
            {
                showArrow = true;
                isExpanded = newHierarchyItemData.isExpanded;
            }
            else
            {
                showArrow = false;
            }
        }

        public class Factory : PlaceholderFactory<HierarchyItemHandler>
        {
        }
    }
}
