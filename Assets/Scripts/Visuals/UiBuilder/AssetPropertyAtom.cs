using Assets.Scripts.EditorState;
using JetBrains.Annotations;
using System;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetPropertyAtom : Atom
    {
        public struct UiPropertyActions<T>
        {
            [CanBeNull]
            public Action<T> OnChange;

            [CanBeNull]
            public Action OnInvalid;

            [CanBeNull]
            public Action<T> OnSubmit;

            [CanBeNull]
            public Action<T> OnAbort;

            public bool Equals(UiPropertyActions<T> other)
            {
                return
                    OnChange == other.OnChange &&
                    OnInvalid == other.OnInvalid &&
                    OnSubmit == other.OnSubmit &&
                    OnAbort == other.OnAbort;
            }
        }

        public new class Data : Atom.Data
        {
            public string propertyName;
            public AssetMetadata assetMetadata;

            public UiPropertyActions<Guid> actions;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherAssetProperty))
                {
                    return false;
                }

                if(propertyName != otherAssetProperty.propertyName)
                {
                    return false;
                }

                if(assetMetadata != otherAssetProperty.assetMetadata)
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

            var newAssetPropertyData = (Data) newData;

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
                assetPropertyHandler.assetBrowserButtonHandler.Init(newAssetPropertyData.assetMetadata, false, null);
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
            StringPropertyAtom.UiPropertyActions<Guid> actions)
        {
            var data = new AssetPropertyAtom.Data
            {
                propertyName = propertyName,
                assetMetadata = assetMetadata,
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}