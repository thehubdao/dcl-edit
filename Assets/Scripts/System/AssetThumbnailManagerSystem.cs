using Assets.Scripts.EditorState;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class AssetThumbnailData : AssetData
    {
        public Texture2D thumbnail;

        public AssetThumbnailData(Guid id, State state, Texture2D thumbnail) : base(id, state)
        {
            this.thumbnail = thumbnail;
        }
    }

    public class AssetThumbnailManagerSystem
    {
        // Dependencies
        private AssetThumbnailGeneratorSystem _assetThumbnailGeneratorSystem;
        private IAssetLoaderSystem[] _assetLoaderSystems;

        [Inject]
        public void Construct(AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem, params IAssetLoaderSystem[] assetLoaderSystems)
        {
            _assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
            _assetLoaderSystems = assetLoaderSystems;
        }

        public AssetThumbnailData GetThumbnailById(Guid id)
        {
            Texture2D thumbnail = null;
            foreach (var loader in _assetLoaderSystems)
            {
                thumbnail = loader.GetThumbnailById(id);
                if (thumbnail != null) break;
            }

            if (thumbnail != null)
            {
                return new AssetThumbnailData(id, AssetData.State.IsAvailable, thumbnail);
            }

            _assetThumbnailGeneratorSystem.Enqueue(id);
            return new AssetThumbnailData(id, AssetData.State.IsLoading, null);
        }

        public void SetThumbnailById(Guid id, Texture2D newThumbnail)
        {
            foreach (IAssetLoaderSystem loaderSystem in _assetLoaderSystems)
            {
                loaderSystem.SetThumbnailById(id, newThumbnail);
            }
        }
    }
}