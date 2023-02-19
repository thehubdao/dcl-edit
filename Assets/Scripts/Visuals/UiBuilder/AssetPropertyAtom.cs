using Assets.Scripts.EditorState;
using System;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetPropertyAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string propertyName;
            public AssetMetadata assetMetadata;
            public Action<Guid> onClick;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherAssetProperty))
                {
                    return false;
                }

                if (propertyName != otherAssetProperty.propertyName)
                {
                    return false;
                }

                if (assetMetadata != otherAssetProperty.assetMetadata)
                {
                    return false;
                }

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
                assetPropertyHandler.assetBrowserButtonHandler.Init(newAssetPropertyData.assetMetadata, false, newAssetPropertyData.onClick);
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
            AssetMetadata assetMetadata,
            Action<Guid> onClick)
        {
            var data = new AssetPropertyAtom.Data
            {
                propertyName = propertyName,
                assetMetadata = assetMetadata,
                onClick = onClick
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}