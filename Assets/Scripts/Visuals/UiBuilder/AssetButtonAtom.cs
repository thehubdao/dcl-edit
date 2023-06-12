using System;
using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetButtonAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public enum TypeIndicator
            {
                None,
                Model,
                Image,
                Scene
            }

            [NotNull]
            public SetValueStrategy<Guid> valueBindStrategy;

            [CanBeNull]
            public LeftClickStrategy leftClick = null;

            [CanBeNull]
            public RightClickStrategy rightClick = null;

            [CanBeNull]
            public DragStrategy dragStrategy = null;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherBtn))
                {
                    return false;
                }

                if (dragStrategy != otherBtn.dragStrategy) return false;
                if (leftClick != otherBtn.leftClick) return false;
                if (rightClick != otherBtn.rightClick) return false;
                if (dragStrategy != otherBtn.dragStrategy) return false;
                return true;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newBtnData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newBtnData.Equals(data))
            {
                // Update data
                var btnHandler = gameObject.gameObject.GetComponent<AssetButtonHandler>();
                btnHandler.Setup(newBtnData.valueBindStrategy, newBtnData.dragStrategy, newBtnData.leftClick, newBtnData.rightClick);

                data = newBtnData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.AssetBrowserButton);
            return atomObject;
        }


        public AssetButtonAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class AssetButtonPanelHelper
    {
        public static AssetButtonAtom.Data AddAssetBrowserButton(
            this PanelAtom.Data panelAtomData,
            // Placeholder Thumbnail Handler
            [NotNull] SetValueStrategy<Guid> valueBindStrategy,
            [CanBeNull] LeftClickStrategy leftClick = null,
            [CanBeNull] RightClickStrategy rightClick = null,
            [CanBeNull] DragStrategy dragStrategy = null
        )
        {
            var data = new AssetButtonAtom.Data
            {
                valueBindStrategy = valueBindStrategy,
                leftClick = leftClick,
                rightClick = rightClick,
                dragStrategy = dragStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}