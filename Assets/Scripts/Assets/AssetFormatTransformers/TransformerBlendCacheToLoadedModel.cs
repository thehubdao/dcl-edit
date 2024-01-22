using System;
using Assets.Scripts.Assets;
using Assets.Scripts.System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBlendCacheToLoadedModel : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private LoadGltfFromFileSystem loadGltfFromFileSystem;

    [Inject]
    private void Construct(LoadGltfFromFileSystem loadGltfFromFileSystem)
    {
        this.loadGltfFromFileSystem = loadGltfFromFileSystem;
    }


    public Type fromType => typeof(AssetFormatBlendCached);
    public Type toType => typeof(AssetFormatLoadedModel);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        Assert.IsTrue(asset.assetType == CommonAssetTypes.AssetType.Model3D);
        Assert.IsTrue(fromFormat is AssetFormatBlendCached);

        var fromBlendCachedFormat = (AssetFormatBlendCached) fromFormat;
        var newLoadedModelFormat = new AssetFormatLoadedModel(fromBlendCachedFormat.hash);

        loadGltfFromFileSystem.LoadGltfFromPath(fromBlendCachedFormat.glbPath, go =>
        {
            if (go == null)
            {
                newLoadedModelFormat.SetError();
                asset.InvokeAssetFormatChanged();
            }
            else
            {
                newLoadedModelFormat.SetModel(go);
                asset.InvokeAssetFormatChanged();
            }
        });

        return newLoadedModelFormat;
    }
}
