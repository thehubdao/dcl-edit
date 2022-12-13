using Assets.Scripts.EditorState;
using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetBrowserFolderAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data
        {
            public AssetHierarchyItem hierarchyItem;
            public ScrollRect scrollViewRect;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherFolder))
                {
                    return false;
                }

                if (hierarchyItem.name != otherFolder.hierarchyItem.name)
                {
                    return false;
                }

                if (scrollViewRect != otherFolder.scrollViewRect)
                {
                    return false;
                }

                return base.Equals(other);
            }
        }

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newFolderData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewAtomGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newFolderData.Equals(data))
            {
                // Update data
                var handler = gameObject.gameObject.GetComponent<AssetBrowserFolderHandler>();
                handler.Init(newFolderData.hierarchyItem, newFolderData.scrollViewRect);
                data = newFolderData;
            }
        }

        protected override AtomGameObject MakeNewAtomGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.AssetBrowserFolder);
            return atomObject;
        }

        protected override void MakeLayoutGroup(PanelAtom.Data newPanelData) { }

        public AssetBrowserFolderAtom(UiBuilder uiBuilder) : base(uiBuilder) { }
    }

    public static class AssetFolderPanelHelper
    {
        public static AssetBrowserFolderAtom.Data AddAssetBrowserFolder(this PanelAtom.Data panelAtomData,
            AssetHierarchyItem hierarchyItem,
            ScrollRect scrollViewRect
            )
        {
            var data = new AssetBrowserFolderAtom.Data
            {
                hierarchyItem = hierarchyItem,
                scrollViewRect = scrollViewRect
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}