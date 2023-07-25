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

            [NotNull]
            public ClickStrategy clickStrategy;

            [CanBeNull]
            public DragStrategy dragStrategy = null;

            [CanBeNull]
            public DropStrategy dropStrategy = null;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherAssetProperty))
                {
                    return false;
                }

                if (propertyName != otherAssetProperty.propertyName) return false;
                if (valueBindStrategy != otherAssetProperty.valueBindStrategy) return false;
                if (clickStrategy != otherAssetProperty.clickStrategy) return false;
                if (dragStrategy != otherAssetProperty.dragStrategy) return false;
                if (dropStrategy != otherAssetProperty.dropStrategy) return false;
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
                    newAssetPropertyData.dropStrategy,
                    newAssetPropertyData.clickStrategy);
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
            [NotNull] SetValueStrategy<Guid> valueBindStrategy,
            [CanBeNull] ClickStrategy clickStrategy = null,
            [CanBeNull] DragStrategy dragStrategy = null,
            [CanBeNull] DropStrategy dropStrategy = null)
        {
            clickStrategy ??= new ClickStrategy();

            var data = new AssetPropertyAtom.Data
            {
                propertyName = propertyName,
                valueBindStrategy = valueBindStrategy,
                clickStrategy = clickStrategy,
                dragStrategy = dragStrategy,
                dropStrategy = dropStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}