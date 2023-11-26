using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;

public class AssetFormatBuilderDownload : CommonAssetTypes.AssetFormat
{
    public override string formatName => "Builder Asset (Downloaded)";
    public override string hash => "";
    public override CommonAssetTypes.Availability availability { get; }
}
