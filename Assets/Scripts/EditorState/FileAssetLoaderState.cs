using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class FileAssetLoaderState
    {
        // Connects an assets Guid to a metadatafile on the filesystem. Allows accessing metadata directly by using a Guid.
        public Dictionary<Guid, AssetMetadataFile> assetMetadataCache = new Dictionary<Guid, AssetMetadataFile>();

        // Contains all asset data that is already cached.
        public Dictionary<Guid, AssetData> assetDataCache = new Dictionary<Guid, AssetData>();
    }


    /// <summary>
    /// Connects a file on the filesystem to an asset data object in the cache.
    /// </summary>
    public class AssetMetadataFile
    {
        public struct Contents
        {
            public AssetMetadata metadata;
            public Texture2D thumbnail;
        }

        public string metadataFilePath;
        public Contents contents;

        public AssetMetadataFile(Contents contents, string path)
        {
            this.contents = contents;
            this.metadataFilePath = path;
        }
        public string AssetFilePath => Path.Combine(Path.GetDirectoryName(metadataFilePath), contents.metadata.assetFilename);
    }

    public class AssetMetadata
    {
        public enum AssetType
        {
            Unknown,
            Model,
            Image
        }

        public string assetFilename;
        public string assetDisplayName => Path.GetFileNameWithoutExtension(assetFilename) ?? "Unkown Asset";
        public Guid assetId;
        public AssetType assetType;
        // public AssetSource source; (Local filesystem, DecentralandBuilder,...)

        new public string ToString() => $"{assetId}; {assetFilename}; {assetType}";
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
