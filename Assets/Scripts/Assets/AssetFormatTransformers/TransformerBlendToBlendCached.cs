using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBlendToBlendCached : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private BlenderExecutor blenderExecutor;
    private PathState pathState;

    [Inject]
    private void Construct(
        BlenderExecutor blenderExecutor,
        PathState pathState)
    {
        this.blenderExecutor = blenderExecutor;
        this.pathState = pathState;
    }


    public Type fromType => typeof(AssetFormatBlendOnDisc);
    public Type toType => typeof(AssetFormatBlendCached);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        Assert.IsTrue(fromFormat is AssetFormatBlendOnDisc);

        var fromBlendFormat = (AssetFormatBlendOnDisc) fromFormat;
        var toFormat = new AssetFormatBlendCached(fromBlendFormat.hash);

        var savePath = Path.Combine(Application.persistentDataPath, "blend_cache", $"{asset.assetId}.glb");
        blenderExecutor.ConvertBlendToGlb(fromBlendFormat.filePath, savePath, actualPath =>
        {
            if (actualPath == null)
            {
                toFormat.SetError();
                asset.InvokeAssetFormatChanged();
            }
            else
            {
                toFormat.SetGlb(actualPath);
                asset.InvokeAssetFormatChanged();
            }
        });

        return toFormat;
    }
}
