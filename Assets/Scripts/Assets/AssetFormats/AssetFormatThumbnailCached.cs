using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetFormatThumbnailCached : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Cached Builder Thumbnail";
    public override string hash => hashInternal.ToString();
    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal = CommonAssetTypes.Availability.Loading;

    private string hashInternal = "";

    public string imagePath { get; private set; } = "";

    public AssetFormatThumbnailCached(string hash)
    {
        hashInternal = hash;
    }

    public void SetImage(string newImagePath)
    {
        imagePath = newImagePath;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public void SetError()
    {
        availabilityInternal = CommonAssetTypes.Availability.Error;
    }
}