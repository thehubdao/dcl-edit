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


        public List<AssetMetadata.AssetType> shownAssetTypes { get; }
        public Sorting sorting { get; private set; }


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
    }
}
