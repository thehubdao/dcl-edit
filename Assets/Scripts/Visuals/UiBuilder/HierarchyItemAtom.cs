using System;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class HierarchyItemAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string name;
            public int level;
            public bool hasChildren;
            public bool isExpanded;
            public TextHandler.TextStyle style;
            public HierarchyItemHandler.UiHierarchyItemActions actions;
            public Action<Vector3> rightClickAction;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is HierarchyItemAtom.Data otherHierarchyItem))
                {
                    return false;
                }

                return
                    name.Equals(otherHierarchyItem.name) &&
                    level.Equals(otherHierarchyItem.level) &&
                    hasChildren.Equals(otherHierarchyItem.hasChildren) &&
                    isExpanded.Equals(otherHierarchyItem.isExpanded) &&
                    style.Equals(otherHierarchyItem.style) &&
                    actions.Equals(otherHierarchyItem.actions) &&
                    rightClickAction.Equals(otherHierarchyItem.rightClickAction);
            }
        }

        protected Data data;

        public override bool Update(Atom.Data newData, int newPosition)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newHierarchyItemData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newHierarchyItemData.Equals(data))
            {
                // Update data
                var hierarchyItemHandler = gameObject.gameObject.GetComponent<HierarchyItemHandler>();

                hierarchyItemHandler.text.text = newHierarchyItemData.name;
                hierarchyItemHandler.indent.offsetMin = new Vector2(20 * newHierarchyItemData.level, 0);
                hierarchyItemHandler.text.textStyle = newHierarchyItemData.style;
                hierarchyItemHandler.actions = newHierarchyItemData.actions;
                hierarchyItemHandler.rightClickHandler.rightClick = newHierarchyItemData.rightClickAction;

                if (newHierarchyItemData.hasChildren)
                {
                    hierarchyItemHandler.showArrow = true;
                    hierarchyItemHandler.isExpanded = newHierarchyItemData.isExpanded;
                }
                else
                {
                    hierarchyItemHandler.showArrow = false;
                }

                data = newHierarchyItemData;
            }

            // Stage 3: Check for changes in Position and Height and update, if it has changed
            if (newPosition != gameObject.position)
            {
                UpdatePositionAndSize(newPosition, gameObject.height);
                posHeightHasChanged = true;
            }

            return posHeightHasChanged;
        }

        private AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.HierarchyItem);
            atomObject.height = 30;
            atomObject.position = -1;
            return atomObject;
        }


        public HierarchyItemAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class HierarchyItemPanelHelper
    {
        public static HierarchyItemAtom.Data AddHierarchyItem(this PanelAtom.Data panelAtomData, string name, int level, bool hasChildren, bool isExpanded, TextHandler.TextStyle textStyle, HierarchyItemHandler.UiHierarchyItemActions actions, Action<Vector3> rightClickAction)
        {
            var data = new HierarchyItemAtom.Data
            {
                name = name,
                level = level,
                hasChildren = hasChildren,
                isExpanded = isExpanded,
                style = textStyle,
                actions = actions,
                rightClickAction = rightClickAction
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}