using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEditor;
using UnityEngine;

public class AssetFormatBlendCached : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Cached Blend File";
    public override string hash => hashInternal.ToString();
    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal = CommonAssetTypes.Availability.Loading;

    private string hashInternal = "";

    public string glbPath { get; private set; } = "";

    public AssetFormatBlendCached(string hash)
    {
        hashInternal = hash;
    }

    public void SetGlb(string newGlbPath)
    {
        glbPath = newGlbPath;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public void SetError()
    {
        availabilityInternal = CommonAssetTypes.Availability.Error;
    }
}
