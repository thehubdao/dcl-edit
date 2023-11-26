using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;

public class AssetFormatBuilderCloud : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Builder Asset";
    public override string hash => "";
    public override CommonAssetTypes.Availability availability => CommonAssetTypes.Availability.Available;

    public Dictionary<string, string> contents;
    public string modelCloudPath;
    public string thumbnailCloudHash;

    public AssetFormatBuilderCloud(Dictionary<string, string> contents, string modelCloudPath, string thumbnailCloudHash)
    {
        this.contents = contents;
        this.modelCloudPath = modelCloudPath;
        this.thumbnailCloudHash = thumbnailCloudHash;
    }
}
