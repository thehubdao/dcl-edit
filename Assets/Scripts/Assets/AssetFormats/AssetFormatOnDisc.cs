using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetFormatOnDisc : CommonAssetTypes.AssetFormat
{
    public override string formatName => "On Disc";
    public override string hash => hashInternal.ToString();
    public override CommonAssetTypes.Availability availability => CommonAssetTypes.Availability.Available;

    private DateTime hashInternal = DateTime.MinValue;

    private string filePath = "";
    private string metaPath = "";

    public AssetFormatOnDisc(string metaPath, string assetPath)
    {
        SetPaths(metaPath, assetPath);
    }

    public void SetPaths(string metaPath, string assetPath)
    {
        this.metaPath = metaPath;
        this.filePath = assetPath;

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
