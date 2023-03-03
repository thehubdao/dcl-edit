using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetBrowserSystem
    {
        public List<AssetMetadata.AssetType> filters => assetBrowserState.shownAssetTypes;

        // Dependencies
        private AssetManagerSystem assetManagerSystem;
        private AssetBrowserState assetBrowserState;
        private EditorEvents editorEvents;

        [Inject]
        public void Construct(AssetManagerSystem assetManagerSystem, AssetBrowserState assetBrowserState, EditorEvents editorEvents)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.assetBrowserState = assetBrowserState;
            this.editorEvents = editorEvents;
        }


        public List<AssetHierarchyItem> GetFilteredAssetHierarchy()
        {
            List<AssetHierarchyItem> hierarchy = assetManagerSystem.GetHierarchy();
            List<AssetHierarchyItem> filteredHierarchy = new List<AssetHierarchyItem>();

            foreach (AssetHierarchyItem item in hierarchy)
            {
                filteredHierarchy.Add(ApplyFilters(item));
            }

            for (int i = 0; i < filteredHierarchy.Count; i++)
            {
                filteredHierarchy[i] = ApplySorting(filteredHierarchy[i]);
            }

            return filteredHierarchy;
        }


        private AssetHierarchyItem ApplyFilters(AssetHierarchyItem hierarchyItem)
        {
            AssetHierarchyItem filteredItem = new AssetHierarchyItem(hierarchyItem.name, hierarchyItem.path);

            foreach (AssetHierarchyItem subdir in hierarchyItem.childDirectories)
            {
                filteredItem.childDirectories.Add(ApplyFilters(subdir));
            }

            foreach (AssetMetadata asset in hierarchyItem.assets)
            {
                // Temporary fix: Image assets are currently disabled because they cannot be used yet.
                if (asset.assetType == AssetMetadata.AssetType.Image) continue;

                if (assetBrowserState.shownAssetTypes.Contains(asset.assetType) || assetBrowserState.shownAssetTypes.Count == 0)
                {
                    filteredItem.assets.Add(asset);
                }
            }

            return filteredItem;
        }


        private AssetHierarchyItem ApplySorting(AssetHierarchyItem hierarchyItem)
        {
            AssetHierarchyItem sortedItem = new AssetHierarchyItem(hierarchyItem.name, hierarchyItem.path);

            switch (assetBrowserState.sorting)
            {
                case AssetBrowserState.Sorting.NameAscending:
                    sortedItem.childDirectories = hierarchyItem.childDirectories.OrderBy(item => item.name).ToList();
                    sortedItem.assets = hierarchyItem.assets.OrderBy(item => item.assetDisplayName).ToList();
                    break;
                case AssetBrowserState.Sorting.NameDescending:
                    sortedItem.childDirectories = hierarchyItem.childDirectories.OrderByDescending(item => item.name).ToList();
                    sortedItem.assets = hierarchyItem.assets.OrderByDescending(item => item.assetDisplayName).ToList();
                    break;
            }

            for (int i = 0; i < sortedItem.childDirectories.Count; i++)
            {
                sortedItem.childDirectories[i] = ApplySorting(sortedItem.childDirectories[i]);
            }

            return sortedItem;
        }


        public void AddFilter(AssetMetadata.AssetType assetType)
        {
            if (assetBrowserState.AddShownType(assetType))
            {
                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
        }


        public void RemoveFilter(AssetMetadata.AssetType assetType)
        {
            if (assetBrowserState.RemoveShownType(assetType))
            {
                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
        }


        public void ChangeSorting(AssetBrowserState.Sorting newSorting)
        {
            if (assetBrowserState.ChangeSorting(newSorting))
            {
                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
        }
    }
}
