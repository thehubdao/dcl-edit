using Assets.Scripts.EditorState;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetBrowserButtonAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public AssetMetadata metadata;
            public bool enableDragAndDrop;
            public Action<Guid> onClick;
            public ScrollRect scrollViewRect;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is AssetBrowserButtonAtom.Data otherBtn))
                {
                    return false;
                }

                if (metadata != otherBtn.metadata)
                {
                    return false;
                }

                if (enableDragAndDrop != otherBtn.enableDragAndDrop) return false;
                if (onClick != otherBtn.onClick) return false;

                if (scrollViewRect != otherBtn.scrollViewRect)
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
                var btnHandler = gameObject.gameObject.GetComponent<AssetBrowserButtonHandler>();
                btnHandler.Init(newBtnData.metadata, newBtnData.enableDragAndDrop, newBtnData.onClick, newBtnData.scrollViewRect);
                data = newBtnData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.AssetBrowserButton);
            return atomObject;
        }


        public AssetBrowserButtonAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class AssetButtonPanelHelper
    {
        public static AssetBrowserButtonAtom.Data AddAssetBrowserButton(
            this PanelAtom.Data panelAtomData,
            AssetMetadata metadata,
            bool enableDragAndDrop,
            Action<Guid> onClick = null,
            ScrollRect scrollViewRect = null
            )
        {
            var data = new AssetBrowserButtonAtom.Data
            {
                metadata = metadata,
                onClick = onClick,
                enableDragAndDrop = enableDragAndDrop,
                scrollViewRect = scrollViewRect
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}