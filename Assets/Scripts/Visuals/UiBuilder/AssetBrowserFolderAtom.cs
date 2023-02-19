using Assets.Scripts.EditorState;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetBrowserFolderAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data
        {
            public AssetHierarchyItem hierarchyItem;

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

                return base.Equals(other);
            }
        }

        public override void Update(Atom.Data newData)
        {
            base.Update(newData);

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
                handler.Initialize(newFolderData.hierarchyItem);
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
        public static AssetBrowserFolderAtom.Data AddAssetBrowserFolder(
            this PanelAtom.Data panelAtomData,
            AssetHierarchyItem hierarchyItem,
            int indentationLevel)
        {
            var data = new AssetBrowserFolderAtom.Data
            {
                hierarchyItem = hierarchyItem,
                indentationLevel = indentationLevel
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}