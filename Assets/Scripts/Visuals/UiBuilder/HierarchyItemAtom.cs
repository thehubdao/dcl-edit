using System;
using Assets.Scripts.SceneState;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
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
            public bool isFirstChild;
            public TextHandler.TextStyle style;
            public bool isPrimarySelected;
            public HierarchyItemHandler.UiHierarchyItemActions actions;

            [CanBeNull]
            public RightClickStrategy rightClickStrategy;
            public DropActions dropActions;
            public DclEntity draggedEntity;

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
                    isPrimarySelected.Equals(otherHierarchyItem.isPrimarySelected) &&
                    isFirstChild.Equals(otherHierarchyItem.isFirstChild) &&
                    style.Equals(otherHierarchyItem.style) &&
                    actions.Equals(otherHierarchyItem.actions) &&
                    (rightClickStrategy?.Equals(otherHierarchyItem.rightClickStrategy) ?? otherHierarchyItem.rightClickStrategy == null) &&
                    dropActions.Equals(otherHierarchyItem.dropActions) &&
                    draggedEntity.Equals(otherHierarchyItem.draggedEntity);
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newHierarchyItemData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newHierarchyItemData.Equals(data))
            {
                // Update data
                var hierarchyItemHandler = gameObject.gameObject.GetComponent<HierarchyItemHandler>();

                hierarchyItemHandler.UpdateHandlers(newHierarchyItemData);

                data = newHierarchyItemData;
            }
        }

        private AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.HierarchyItem);
            return atomObject;
        }


        public HierarchyItemAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class HierarchyItemPanelHelper
    {
        public static HierarchyItemAtom.Data AddHierarchyItem(this PanelAtom.Data panelAtomData, string name, int level,
            bool hasChildren, bool isExpanded, bool isFirstChild, TextHandler.TextStyle textStyle, bool isPrimarySelected,
            HierarchyItemHandler.UiHierarchyItemActions actions,
            Action<GameObject> dropActionUpper, Action<GameObject> dropActionMiddle, Action<GameObject> dropActionLower, RightClickStrategy rightClickStrategy = null,
            DclEntity entity = null)
        {
            var data = new HierarchyItemAtom.Data
            {
                name = name,
                level = level,
                hasChildren = hasChildren,
                isExpanded = isExpanded,
                isFirstChild = isFirstChild,
                style = textStyle,
                isPrimarySelected = isPrimarySelected,
                actions = actions,
                rightClickStrategy = rightClickStrategy,
                dropActions =
                {
                    dropActionUpper = dropActionUpper,
                    dropActionMiddle = dropActionMiddle,
                    dropActionLower = dropActionLower,
                },
                draggedEntity = entity
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }

    public struct DropActions
    {
        public Action<GameObject> dropActionUpper;
        public Action<GameObject> dropActionMiddle;
        public Action<GameObject> dropActionLower;
    }
}