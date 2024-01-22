using Assets.Scripts.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetFormatBlendOnDisc : CommonAssetTypes.AssetFormat, IAssetFormatOnDisc
{
    public override string formatName => "Blend On Disc";
    public override string hash => hashInternal.ToString();
    public override CommonAssetTypes.Availability availability => CommonAssetTypes.Availability.Available;

    private DateTime hashInternal = DateTime.MinValue;

    private string filePathInternal = "";
    private string metaPathInternal = "";

    public string filePath => filePathInternal;
    public string metaPath => metaPathInternal;

    public AssetFormatBlendOnDisc(string metaPath, string assetPath)
    {
        SetPaths(metaPath, assetPath);
    }

    public void SetPaths(string metaPath, string assetPath)
    {
        this.metaPathInternal = metaPath;
        this.filePathInternal = assetPath;

        UpdateHash();
    }

    /**
     * Find the last change of the asset and saves it as the hash
     * <returns>true, if the hash has changed</returns>
     */
    public bool UpdateHash()
    {
        var lastWriteTime = File.GetLastWriteTime(filePath);

        if (lastWriteTime == hashInternal) return false;

        hashInternal = lastWriteTime;
        return true;
    }
}
