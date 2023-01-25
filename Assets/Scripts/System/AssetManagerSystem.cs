using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetManagerSystem
    {
        // Dependencies
        private IAssetLoaderSystem[] _assetLoaderSystems;

        [Inject]
        public void Construct(params IAssetLoaderSystem[] assetLoaderSystems)
        {
            _assetLoaderSystems = assetLoaderSystems;
        }

        public void CacheAllAssetMetadata()
        {
            ClearModelCache();
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                loaderSystem.ClearAllData();
                loaderSystem.CacheAllAssetMetadata();
            }
        }

        public AssetMetadata GetMetadataById(Guid id)
        {
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                var result = loaderSystem.GetMetadataById(id);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public AssetData GetDataById(Guid id)
        {
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                var result = loaderSystem.GetDataById(id);
                if (result != null)
                {
                    return result;
                }
            }

            return new AssetData(id, AssetData.State.IsError);
        }

        public IEnumerable<Guid> GetAllAssetIds()
        {
            IEnumerable<Guid> result = new List<Guid>();
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                result = result.Concat(loaderSystem.GetAllAssetIds());
            }
            return result;
        }

        public List<AssetHierarchyItem> GetHierarchy()
        {
            List<AssetHierarchyItem> hierarchy = new List<AssetHierarchyItem>();

            foreach (var loaderSystem in _assetLoaderSystems)
            {
                hierarchy.Add(loaderSystem.GetHierarchy());
            }

            return hierarchy;
        }

        private void ClearModelCache()
        {
            GameObject modelCache = GameObject.Find("ModelCache");
            if (modelCache != null)
            {
                foreach (Transform child in modelCache.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        public string CopyAssetTo(Guid id, string destPath)
        {
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                var result = loaderSystem.CopyAssetTo(id);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}