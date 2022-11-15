using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.System
{
    public interface IAssetLoaderSystem
    {
        void CacheAllAssetMetadata();
        IEnumerable<Guid> GetAllAssetIds();
        AssetMetadata GetMetadataById(Guid id);
        AssetThumbnail GetThumbnailById(Guid id);
        AssetData GetDataById(Guid id);
    }

    public class AssetMetadata
    {
        public enum AssetType
        {
            Unknown,
            Model,
            Image
        }

        public AssetMetadata(string assetDisplayName, Guid assetId, AssetType assetType)
        {
            this.assetDisplayName = assetDisplayName;
            this.assetId = assetId;
            this.assetType = assetType;
        }

        public string assetDisplayName { get; }
        public Guid assetId { get; }
        public AssetType assetType { get; }
    }

    public class AssetThumbnail
    {
        public Guid Id;

        public AssetData.State State;

        public Texture2D Texture;
    }

    public class AssetData
    {
        public Guid id;

        public enum State
        {
            IsAvailable,
            IsLoading,
            IsError
        }

        public State state;

        public AssetData(Guid id, State state)
        {
            this.id = id;
            this.state = state;
        }
    }

    public class ModelAssetData : AssetData
    {
        public GameObject data;

        public ModelAssetData(Guid id, GameObject data) : base(id, State.IsAvailable)
        {
            this.data = data;
        }
    }

    public class ImageAssetData : AssetData
    {
        public Texture2D data;

        public ImageAssetData(Guid id, Texture2D data) : base(id, State.IsAvailable)
        {
            this.data = data;
        }
    }
}