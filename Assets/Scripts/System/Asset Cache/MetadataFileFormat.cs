using Assets.Scripts.EditorState;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MetadataFileFormat : AssetFormat, IThumbnailConvertible
{
    /// <summary>
    /// Information about the asset to which this metadata file belongs to.
    /// </summary>
    public struct AssetInfo
    {
        public Guid assetId;
        public string assetFilename;
        public AssetMetadata.AssetType assetType;
        public string assetDisplayName;

        public AssetInfo(Guid assetId, string assetFilename, string assetDisplayName, AssetMetadata.AssetType assetType)
        {
            this.assetId = assetId;
            this.assetFilename = assetFilename;
            this.assetType = assetType;
            this.assetDisplayName = assetDisplayName;
        }
    }

    /// <summary>
    /// The content of the metadata file. This is specified in a separate struct, so that only 
    /// the contents of the file are generated during serialization (without the file path).
    /// </summary>
    public struct Contents
    {
        public AssetInfo metadata;
        public string dclEditVersionNumber;
        public string thumbnail;

        public Contents(AssetInfo metadata, string dclEditVersionNumber, Texture2D thumbnail = null)
        {
            this.metadata = metadata;
            this.dclEditVersionNumber = dclEditVersionNumber;
            this.thumbnail =
                thumbnail == null ?
                "" :
                Convert.ToBase64String(thumbnail.EncodeToPNG());
        }
    }

    public string pathToMetadataFile;
    public Contents contents;
    public string assetFilePath => Path.Combine(Path.GetDirectoryName(pathToMetadataFile)!, contents.metadata.assetFilename);

    public MetadataFileFormat(Guid id, string pathToMetadataFile, Contents contents) : base(id)
    {
        this.pathToMetadataFile = pathToMetadataFile;
        this.contents = contents;
    }

    public Task<ThumbnailFormat> ConvertToThumbnailFormat()
    {
        if (!string.IsNullOrEmpty(contents.thumbnail))
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(Convert.FromBase64String(contents.thumbnail));
            // No await needed because LoadImage isn't asynchronous. The task is therefore completed immediately.
            var result = new ThumbnailFormat(id, tex);
            return Task.FromResult(result);
        }
        return Task.FromResult<ThumbnailFormat>(null);
    }
}
