using System.Collections.Generic;

namespace Assets.Scripts.EditorState
{
    public class AssetBrowserState
    {
        public enum Sorting
        {
            NameAscending,
            NameDescending
        }


        public List<AssetMetadata.AssetType> shownAssetTypes { get; private set; }
        public Sorting sorting { get; private set; }
        public List<string> expandedFoldersPaths = new List<string>();

        // This is a temporary solution to allow a dialog setting a custom filter. While the dialog
        // is open, the filters set by the user will be stored here. After the dialog is completed
        // the filters are restored. In the future, each instance of the asset browser should have
        // its own state.
        public List<AssetMetadata.AssetType> temporaryShownAssetTypesOverride;

        public AssetBrowserState()
        {
            shownAssetTypes = new List<AssetMetadata.AssetType>();
        }


        public bool AddShownType(AssetMetadata.AssetType type)
        {
            if (shownAssetTypes.Contains(type))
            {
                return false;
            }

            shownAssetTypes.Add(type);
            return true;
        }

        public bool RemoveShownType(AssetMetadata.AssetType type)
        {
            return shownAssetTypes.Remove(type);
        }

        public bool ChangeSorting(Sorting newSorting)
        {
            if (sorting == newSorting)
            {
                return false;
            }

            sorting = newSorting;
            return true;
        }



        // These two methods are also temporary to allow storing the asset browser filters until each
        // instance of the asset browser gets its own state.
        public void StoreShownTypesTemp()
        {
            temporaryShownAssetTypesOverride = shownAssetTypes;
            shownAssetTypes = new List<AssetMetadata.AssetType>();
        }
        public void RestoreShownTypes()
        {
            shownAssetTypes = temporaryShownAssetTypesOverride;
            temporaryShownAssetTypesOverride = new List<AssetMetadata.AssetType>();
        }
    }
}
