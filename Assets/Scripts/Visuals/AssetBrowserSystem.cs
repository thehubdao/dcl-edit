using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Zenject;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class AssetBrowserSystem
    {
        public List<AssetMetadata.AssetType> filters => assetBrowserState.shownAssetTypes;

        // Dependencies
        private AssetManagerSystem assetManagerSystem;
        private AssetBrowserState assetBrowserState;
        private EditorEvents editorEvents;
        private CameraState cameraState;
        private AddEntitySystem addEntitySystem;
        private InputHelper inputHelper;

        [Inject]
        public void Construct(
            AssetManagerSystem assetManagerSystem,
            AssetBrowserState assetBrowserState,
            EditorEvents editorEvents,
            CameraState cameraState,
            AddEntitySystem addEntitySystem,
            InputHelper inputHelper)
        {
            this.assetManagerSystem = assetManagerSystem;
            this.assetBrowserState = assetBrowserState;
            this.editorEvents = editorEvents;
            this.cameraState = cameraState;
            this.addEntitySystem = addEntitySystem;
            this.inputHelper = inputHelper;
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

        public void AddAssetToSceneInViewportCenter(AssetMetadata asset)
        {
            Ray ray = new Ray(cameraState.Position, cameraState.Forward);
            var position = Physics.Raycast(ray, out RaycastHit hit, 50) ?
                hit.point :
                ray.GetPoint(10);

            addEntitySystem.AddModelAssetEntityAsCommand(asset.assetDisplayName, asset.assetId, position);
        }

        public void AddAssetToSceneAtMousePositionInViewport(AssetMetadata asset)
        {
            var position = inputHelper.GetMousePositionInScene();

            switch (asset.assetType)
            {
                case AssetMetadata.AssetType.Unknown:
                    throw new ArgumentOutOfRangeException();
                case AssetMetadata.AssetType.Model:
                    addEntitySystem.AddModelAssetEntityAsCommand(asset.assetDisplayName, asset.assetId, position);
                    break;
                case AssetMetadata.AssetType.Scene:
                    addEntitySystem.AddSceneAssetEntityAsCommand(asset.assetDisplayName, asset.assetId, position);
                    break;
                case AssetMetadata.AssetType.Image:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
