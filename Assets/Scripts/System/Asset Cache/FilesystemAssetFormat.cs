using System;

/// <summary>
/// Represents an asset on the filesystem.
/// </summary>
public abstract class FilesystemAssetFormat : AssetFormat
{
    public string assetFilename;
    public string assetDisplayName;
    public string dclEditVersionNumber;
    public string thumbnail;

    public FilesystemAssetFormat(Guid id, string assetFilename, string assetDisplayname, string dclEditVersionNumber, string thumbnail) : base(id)
    {
        this.assetFilename = assetFilename;
        this.assetDisplayName = assetDisplayname;
        this.dclEditVersionNumber = dclEditVersionNumber;
        this.thumbnail = thumbnail;
    }
}
