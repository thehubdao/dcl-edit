using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


/**
 * This format will build all assets into a common folder inside the Decentraland Project.
 * The folder is a flat directory of all assets.
 * All assets are numbered from 1 to n.
 * The assets have an extension based on their type.
 * E.g. Image assets will have the names "1.png", "2.png", "3.png", ...
 */
public class AssetFormatBuildForDecentraland : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Build For Decentraland";
    public override string hash { get; }
    public override CommonAssetTypes.Availability availability => availabilityInternal;

    private CommonAssetTypes.Availability availabilityInternal;

    public string buildPath;

    private static Dictionary<string, int> assetCounter = new();

    public AssetFormatBuildForDecentraland(string hash, string buildPath)
    {
        this.hash = hash;
        this.buildPath = buildPath;
        availabilityInternal = CommonAssetTypes.Availability.Available;
    }

    public static string GetNextName(string extension)
    {
        var normalizedExtension = NormalizeExtension(extension);

        if (assetCounter.ContainsKey(normalizedExtension))
        {
            assetCounter[normalizedExtension]++;
        }
        else
        {
            assetCounter.Add(normalizedExtension, 1);
        }

        return $"{assetCounter[normalizedExtension]}.{normalizedExtension}";
    }

    private static string NormalizeExtension(string extension)
    {
        extension = extension.TrimStart(' ', '.');

        return extension switch
        {
            "png" => "png",
            "PNG" => "png",
            "jpg" => "jpg",
            "JPG" => "jpg",
            "jpeg" => "jpg",
            "JPEG" => "jpg",
            "glb" => "glb",
            "GLB" => "glb",

            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, "the extension was not recognized")
        };
    }
}
