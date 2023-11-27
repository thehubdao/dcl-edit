using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;

public class AssetFormatLoadedModel : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Loaded model";
    public override string hash { get; }

    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal;

    public GameObject modelTemplate;

    public AssetFormatLoadedModel(string hash)
    {
        this.hash = hash;
        availabilityInternal = CommonAssetTypes.Availability.Loading;
    }

    public void SetModel(GameObject modelTemplate)
    {
        this.modelTemplate = modelTemplate;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public void SetError()
    {
        availabilityInternal = CommonAssetTypes.Availability.Error;
    }
}
