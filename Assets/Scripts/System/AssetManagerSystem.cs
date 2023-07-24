using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetManagerSystem
    {
        // Dependencies
        private IAssetLoaderSystem[] _assetLoaderSystems;
        private AssetCacheSystem assetCacheSystem;

        [Inject]
        public void Construct(AssetCacheSystem assetCacheSystem, params IAssetLoaderSystem[] assetLoaderSystems)
        {
            this.assetCacheSystem = assetCacheSystem;
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

        public AssetMetadata GetMetadataById(Guid id) => assetCacheSystem.GetMetadata(id);

        /// <summary>
        /// Returns the data of the asset with the given ID. If needed, a CancellationTokenSource can be used 
        /// to abort the request.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public async Task<AssetData> GetDataById(Guid id)
        {
            GameObject model = await assetCacheSystem.GetLoadedModel(id);
            if (model == null) return null;
            return new ModelAssetData(id, model);
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

        public async Task<Texture2D> GetThumbnailById(Guid id) => await assetCacheSystem.GetThumbnail(id);

        public async Task<Sprite> GetThumbnailSpriteById(Guid id) => await assetCacheSystem.GetThumbnailSprite(id);

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

        public async Task<string> CopyAssetTo(Guid id, string destPath)
        {
            foreach (var loaderSystem in _assetLoaderSystems)
            {
                var result = await loaderSystem.CopyAssetTo(id);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}