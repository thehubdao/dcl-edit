using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;

public class AssetFormatBuilderCloud : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Builder Asset";
    public override string hash => "";
    public override CommonAssetTypes.Availability availability => CommonAssetTypes.Availability.Available;

    public bool isPrimaryModel;
    public Dictionary<string, string> contents;
    public string cloudPath;
    public string cloudHash;
    public string thumbnailCloudHash;

    public AssetFormatBuilderCloud(Dictionary<string, string> contents, string cloudPath, string thumbnailCloudHash)
    {
        isPrimaryModel = true;
        this.contents = contents;
        this.cloudPath = cloudPath;
        this.cloudHash = contents[cloudPath];
        this.thumbnailCloudHash = thumbnailCloudHash;
    }

    public AssetFormatBuilderCloud(string cloudHash, string cloudPath)
    {
        isPrimaryModel = false;
        contents = null;
        thumbnailCloudHash = null;
        this.cloudPath = cloudPath;
        this.cloudHash = cloudHash;
    }
}
