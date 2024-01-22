using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using UnityEngine;

public class AssetFormatBuilderDownload : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Builder Asset (Downloaded)";
    public override string hash => "";
    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal = CommonAssetTypes.Availability.Loading;

    [CanBeNull]
    public string basePath { get; private set; }

    public void SetBasePath(string basePath)
    {
        this.basePath = basePath;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }
}
