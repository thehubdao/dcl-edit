using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
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

        public virtual string assetDisplayName { get; }
        public Guid assetId { get; }
        public AssetType assetType { get; }
    }

    public class AssetThumbnail
    {
        public AssetThumbnail(Guid id, AssetData.State state, Texture2D texture)
        {
            this.id = id;
            this.state = state;
            this.texture = texture;
        }

        public Guid id;

        public AssetData.State state;

        public Texture2D texture;
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