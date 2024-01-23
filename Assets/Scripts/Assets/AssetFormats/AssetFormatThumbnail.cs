using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetFormatThumbnail : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Thumbnail";
    public override string hash => hashInternal;
    public override CommonAssetTypes.Availability availability => availabilityInternal;


    private string hashInternal;
    private CommonAssetTypes.Availability availabilityInternal;

    public Sprite thumbnail { get; private set; } = null;

    public AssetFormatThumbnail(string hash)
    {
        hashInternal = hash;
        availabilityInternal = CommonAssetTypes.Availability.Loading;
    }

    public void SetThumbnail(Sprite thumbnail)
    {
        this.thumbnail = thumbnail;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public void SetError()
    {
        thumbnail = null;
        availabilityInternal = CommonAssetTypes.Availability.Error;
    }
}
