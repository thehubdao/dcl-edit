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
            public SetValueStrategy<string> name;
            public int level;
            public bool hasChildren;
            public bool isExpanded;
            public bool isFirstChild;
            public TextHandler.TextStyle style;
            public bool isPrimarySelected;

            [NotNull]
            public ClickStrategy clickArrowStrategy;

            [NotNull]
            public ClickStrategy clickTextStrategy;

            [CanBeNull]
            public DropStrategy dropStrategyUpper;

            [CanBeNull]
            public DropStrategy dropStrategyMiddle;

            [CanBeNull]
            public DropStrategy dropStrategyLower;

            [CanBeNull]
            public DragStrategy dragStrategy;


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
                    (dropStrategyUpper?.Equals(otherHierarchyItem.dropStrategyUpper) ?? otherHierarchyItem.dropStrategyUpper == null) &&
                    (dropStrategyMiddle?.Equals(otherHierarchyItem.dropStrategyMiddle) ?? otherHierarchyItem.dropStrategyMiddle == null) &&
                    (dropStrategyLower?.Equals(otherHierarchyItem.dropStrategyLower) ?? otherHierarchyItem.dropStrategyLower == null) &&
                    (dragStrategy?.Equals(otherHierarchyItem.dragStrategy) ?? otherHierarchyItem.dragStrategy == null);
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
        public static HierarchyItemAtom.Data AddHierarchyItem(
            this PanelAtom.Data panelAtomData,
            SetValueStrategy<string> name,
            int level,
            bool hasChildren,
            bool isExpanded,
            bool isFirstChild,
            TextHandler.TextStyle textStyle,
            bool isPrimarySelected,
            ClickStrategy clickArrowStrategy = null,
            ClickStrategy clickTextStrategy = null,
            DropStrategy dropStrategyUpper = null,
            DropStrategy dropStrategyMiddle = null,
            DropStrategy dropStrategyLower = null,
            DragStrategy dragStrategy = null)
        {
            clickTextStrategy ??= new ClickStrategy();
            clickArrowStrategy ??= new ClickStrategy();

            var data = new HierarchyItemAtom.Data
            {
                name = name,
                level = level,
                hasChildren = hasChildren,
                isExpanded = isExpanded,
                isFirstChild = isFirstChild,
                style = textStyle,
                isPrimarySelected = isPrimarySelected,
                clickArrowStrategy = clickArrowStrategy,
                clickTextStrategy = clickTextStrategy,
                dragStrategy = dragStrategy,
                dropStrategyUpper = dropStrategyUpper,
                dropStrategyMiddle = dropStrategyMiddle,
                dropStrategyLower = dropStrategyLower
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}