using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var loaderSystem in _assetLoaderSystems)
            {
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
            return null;
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
    }
}