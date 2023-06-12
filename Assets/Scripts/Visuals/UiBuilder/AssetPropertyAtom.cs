using JetBrains.Annotations;
using System;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetPropertyAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string propertyName;

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
                if (!(other is Data otherAssetProperty))
                {
                    return false;
                }

                if (propertyName != otherAssetProperty.propertyName) return false;
                if (valueBindStrategy != otherAssetProperty.valueBindStrategy) return false;
                if (leftClick != otherAssetProperty.leftClick) return false;
                if (rightClick != otherAssetProperty.rightClick) return false;
                if (dragStrategy != otherAssetProperty.dragStrategy) return false;
                return true;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newAssetPropertyData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newAssetPropertyData.Equals(data))
            {
                var assetPropertyHandler = gameObject.gameObject.GetComponent<AssetPropertyHandler>();
                assetPropertyHandler.propertyNameText.text = newAssetPropertyData.propertyName;
                assetPropertyHandler.assetButtonHandler.Setup(
                    newAssetPropertyData.valueBindStrategy,
                    newAssetPropertyData.dragStrategy,
                    newAssetPropertyData.leftClick,
                    newAssetPropertyData.rightClick);
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.AssetPropertyInput);
            return atomObject;
        }


        public AssetPropertyAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class AssetPropertyPanelHelper
    {
        public static AssetPropertyAtom.Data AddAssetProperty(
            this PanelAtom.Data panelAtomData,
            string propertyName,
            SetValueStrategy<Guid> valueBindStrategy,
            LeftClickStrategy leftClick = null,
            RightClickStrategy rightClick = null,
            DragStrategy dragStrategy = null)
        {
            var data = new AssetPropertyAtom.Data
            {
                propertyName = propertyName,
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