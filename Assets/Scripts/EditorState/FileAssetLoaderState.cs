using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    //public class FileAssetLoaderState
    //{
    //    // Connects an assets Guid to a metadatafile on the filesystem. Allows accessing metadata directly by using a Guid.
    //    public Dictionary<Guid, AssetMetadataFile> assetMetadataCache = new Dictionary<Guid, AssetMetadataFile>();
    //
    //    // Contains all asset data that is already cached.
    //    public Dictionary<Guid, AssetData> assetDataCache = new Dictionary<Guid, AssetData>();
    //
    //    public AssetHierarchyItem assetHierarchy = new AssetHierarchyItem();
    //}
    //
    //
    ///// <summary>
    ///// Connects a file on the filesystem to an asset data object in the cache.
    ///// </summary>
    //public class AssetMetadataFile
    //{
    //    public struct Contents
    //    {
    //        public Contents(MetaContents metadata, [CanBeNull] Texture2D thumbnail)
    //        {
    //            this.metadata = metadata;
    //            dclEditVersionNumber = Application.version;
    //
    //            this.thumbnail =
    //                thumbnail == null ?
    //                    "" :
    //                    Convert.ToBase64String(thumbnail.EncodeToPNG());
    //        }
    //
    //        public MetaContents metadata;
    //        public string dclEditVersionNumber;
    //        public string thumbnail;
    //    }
    //
    //    public struct MetaContents
    //    {
    //        public MetaContents(string assetFilename, Guid assetId, AssetMetadata.AssetType assetType, string assetDisplayName)
    //        {
    //            this.assetFilename = assetFilename;
    //            this.assetId = assetId;
    //            this.assetType = assetType;
    //            this.assetDisplayName = assetDisplayName;
    //        }
    //
    //        public string assetFilename;
    //        public Guid assetId;
    //        public AssetMetadata.AssetType assetType;
    //        public string assetDisplayName;
    //    }
    //
    //    [CanBeNull]
    //    public Texture2D thumbnail;
    //
    //    public string metadataFilePath;
    //
    //    //public Contents contents;
    //    public string assetFilename;
    //    public AssetMetadata assetMetadata;
    //
    //    public AssetMetadataFile(Contents contents, string path)
    //    {
    //        //this.contents = contents;
    //        assetFilename = contents.metadata.assetFilename;
    //
    //        if (!string.IsNullOrEmpty(contents.thumbnail))
    //        {
    //            thumbnail = new Texture2D(2, 2);
    //            thumbnail.LoadImage(Convert.FromBase64String(contents.thumbnail));
    //        }
    //
    //        assetMetadata = new AssetMetadata(contents.metadata.assetDisplayName, contents.metadata.assetId, contents.metadata.assetType);
    //
    //        metadataFilePath = path;
    //    }
    //
    //    public AssetMetadataFile(string metadataFilePath, string assetFilename, AssetMetadata assetMetadata)
    //    {
    //        this.thumbnail = null;
    //        this.metadataFilePath = metadataFilePath;
    //        this.assetFilename = assetFilename;
    //        this.assetMetadata = assetMetadata;
    //    }
    //
    //    public string assetFilePath => Path.Combine(Path.GetDirectoryName(metadataFilePath)!, assetFilename);
    //}
}
