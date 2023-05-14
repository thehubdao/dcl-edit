using Assets.Scripts.EditorState;
using System;
using System.Diagnostics;
using UnityEngine.UI;

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

            public string assetName;
            public TypeIndicator typeIndicator = TypeIndicator.None;
            public LeftClickStrategy leftClick = null;
            public RightClickStrategy rightClick = null;
            public DragStrategy dragStrategy = null;
            // Placeholder Thumbnail Handler

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherBtn))
                {
                    return false;
                }

                if (assetName != otherBtn.assetName) return false;
                if (typeIndicator != otherBtn.typeIndicator) return false;
                if (dragStrategy != otherBtn.dragStrategy) return false;
                if (leftClick != otherBtn.leftClick) return false;
                if (rightClick != otherBtn.rightClick) return false;
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
                btnHandler.Init(newBtnData.assetName, newBtnData.typeIndicator, newBtnData.leftClick, newBtnData.rightClick, newBtnData.dragStrategy);
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
            string assetName,
            AssetMetadata.AssetType assetType = AssetMetadata.AssetType.Unknown,
            // Placeholder Thumbnail Handler
            LeftClickStrategy leftClick = null,
            RightClickStrategy rightClick = null,
            DragStrategy dragStrategy = null
        )
        {
            var typeIndicator = assetType switch
            {
                AssetMetadata.AssetType.Unknown => AssetButtonAtom.Data.TypeIndicator.None,
                AssetMetadata.AssetType.Model => AssetButtonAtom.Data.TypeIndicator.Model,
                AssetMetadata.AssetType.Image => AssetButtonAtom.Data.TypeIndicator.Image,
                AssetMetadata.AssetType.Scene => AssetButtonAtom.Data.TypeIndicator.Scene,
                _ => throw new Exception($"Unknown asset type: {assetType}")
            };

            var data = new AssetButtonAtom.Data
            {
                assetName = assetName,
                typeIndicator = typeIndicator,
                leftClick = leftClick,
                rightClick = rightClick,
                dragStrategy = dragStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}